using GrpcCodeGenerator.Configs;
using GrpcCodeGenerator.Exceptions;
using GrpcCodeGenerator.Extentions;
using GrpcCodeGenerator.InfoModels;
using GrpcCodeGenerator.SyntaxReceivers;
using GrpcCodeGenerator.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace GrpcCodeGenerator.Generators
{
    /// <summary>
    /// Класс, генерирующий реализацию Grpc-сервиса, используя классы шаблоны помеченные некоторыми атрибутами
    /// </summary>
    [Generator]
    public class GrpcImplementationGenerator : ISourceGenerator
    {
        private GrpcMappingClassSyntaxReceiver? _receiver;

        /// <summary>
        /// Содержит в себе: <br/><br/>
        /// Key: Имя модели <br/>
        /// Value: Имена свойств модели
        /// </summary>
        private Dictionary<string, List<string>> _grpcClassAndProperties = new Dictionary<string, List<string>>();

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new GrpcMappingClassSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            _receiver = context.SyntaxContextReceiver as GrpcMappingClassSyntaxReceiver;

            if (_receiver?.GrpcBaseClassInfo?.GrpcBaseClass == null || !_receiver!.MappingClasses.Any())
                return;

            var compilation = context.Compilation;

            foreach (var classSyntax in _receiver.MappingClasses!)
            {
                var methodNodes = classSyntax.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>();

                CheckValidImplementation(methodNodes);

                var semanticModelOfMethod = compilation.GetSemanticModel(classSyntax.SyntaxTree);
                string className          = classSyntax.Identifier.Text;

                var result = GenerateByMethodsInfo(methodNodes, semanticModelOfMethod, className, context.Compilation);

                context.AddSource($"{className}.g.cs", SourceText.From(result, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Генерация строки, содержащей в себе код нового класса, расширяющего Grpc-сервис по шаблону
        /// </summary>
        private string GenerateByMethodsInfo(IEnumerable<MethodDeclarationSyntax> methodNodes, SemanticModel semanticModel, string className, Compilation compilation)
        {
            var stringBuilderFields                = new StringBuilder();
            var stringBuilderConstructorParameters = new StringBuilder();
            var stringBuilderConstructorBody       = new StringBuilder();
            var stringBuilderClassBodyResult       = new StringBuilder();
            var stringBuilderMethods               = new StringBuilder();

            var servicesHashSet            = new HashSet<string>();
            var servicesNamespaceHashSet   = new HashSet<string>();

            foreach (var methodNode in methodNodes)
            {
                if (!methodNode.AttributeLists.Any())
                    continue;

                string methodName  = methodNode.Identifier.Text;
                var methodSymb     = semanticModel.GetDeclaredSymbol(methodNode);
                var attributeDatas = methodSymb?.GetAttributes();

                //Поиск атрибута [GrpcInterface] у метода
                var grpcInterfaceAttr = attributeDatas?.FirstOrDefault(x => x.AttributeClass?.Name == Config.GRPC_ATTR_INTERFACE_NAME);

                if (grpcInterfaceAttr == null)
                    continue;

                var attrConstrArgs           = grpcInterfaceAttr.ConstructorArguments;
                var attrFields               = grpcInterfaceAttr.NamedArguments;
                var serviceArgument          = attrConstrArgs[0];
                var serviceArgumentNamespace = serviceArgument.GetNamespace();

                //Добавление пространства имён сервиса, для последующего включения его в код, если такой существует и ранее не был найден.
                if (!string.IsNullOrWhiteSpace(serviceArgumentNamespace) && !servicesNamespaceHashSet.Contains(serviceArgumentNamespace!))
                {
                    servicesNamespaceHashSet.Add(serviceArgumentNamespace!);
                }

                string serviceName    = serviceArgument.GetShortName()!; //В данном случае, возвращаемая строка не будет нулевой, т.к. не возможно использовать атрибут без параметра 
                string grpcMethodName = methodName;

                //Имеет ли атрибут параметр, отвечающий за название метода сервиса, который будет вызываться в методе
                if (attrFields.Any())
                {
                    grpcMethodName = attrFields[0].Value.Value.ToString();
                }

                //Добавление интерфейса, который должен находится в зависимостях (генерация: поля / параметра конструктора / присваивание полю параметр)
                if (!servicesHashSet.Contains(serviceName))
                {
                    stringBuilderFields.AppendFormat(GrpcTemplates.FieldsTemplate, serviceName);
                    stringBuilderConstructorParameters.AppendFormat(GrpcTemplates.GrpcConstructorParameterTemplate, serviceName);
                    stringBuilderConstructorBody.AppendFormat(GrpcTemplates.ServiceItemTemplate, serviceName);

                    servicesHashSet.Add(serviceName);
                }

                //Поиск атрибута [GrpcStreamMapping] у метода
                var grpcStreamAttrs = attributeDatas?.Where(x => x.AttributeClass?.Name == Config.GRPC_ATTR_STREAM_MAPPING_NAME);

                //При отсутствии атрибута [GrpcStreamMapping], создаётся обычный метод. Иначе создаётся стриминговый метод
                if (!grpcStreamAttrs.Any())
                {
                    stringBuilderMethods.Append(GenerateMethods(serviceName, methodName, grpcMethodName));
                }
                else
                {
                    //Получение пространства имён классов и интерфейсов, которые содержат параметры атрибута
                    //П.с. Более оптимальным было бы использование вложенных циклов, но для красоты обернул всё в LINQ
                    var namespacesForGeneration = grpcStreamAttrs.SelectMany(x => x.NamedArguments.Where(x => 
                        {
                            string? serviceNameSpace = x.Value.GetNamespace();

                            if (string.IsNullOrWhiteSpace(serviceNameSpace) || servicesNamespaceHashSet.Contains(serviceNameSpace!))
                                return false;

                            return true;
                        }).Select(x => x.Value.GetNamespace()))
                        .Distinct()
                        .ToList();

                    foreach (var namespaceForGeneration in namespacesForGeneration)
                    {
                        servicesNamespaceHashSet.Add(namespaceForGeneration!);
                    }

                    stringBuilderMethods.Append(GenerateStreamMethods(serviceName, methodName, grpcMethodName, grpcStreamAttrs!, compilation));
                }
            }

            stringBuilderConstructorParameters.Length -= 2; //Удаляем последние два символа, которые будут в последнем параметре из-за шаблона

            string resultConstructorParameters = string.Format(GrpcTemplates.PublicMethodHeadTemplate, className, stringBuilderConstructorParameters);
            string resultConstructorBody       = string.Format(GrpcTemplates.MethodBodyTemplate, stringBuilderConstructorBody);

            stringBuilderClassBodyResult.Append(stringBuilderFields);
            stringBuilderClassBodyResult.Append(resultConstructorParameters);
            stringBuilderClassBodyResult.Append(resultConstructorBody);
            stringBuilderClassBodyResult.Append(stringBuilderMethods);

            return string.Format(GrpcTemplates.GrpcClassTemplate, GenerateUsings(servicesNamespaceHashSet), _receiver!.GrpcBaseClassInfo!.Namespace, className, stringBuilderClassBodyResult);
        }

        /// <summary>
        /// Генерация строки, содержащей код обычного метода
        /// </summary>
        private string GenerateMethods(string serviceName, string methodName, string grpcMethodName)
        {
            var stringBuilderArgs = new StringBuilder();
            
            foreach(var argName in _receiver!.GrpcMethodRequestInfo!.FieldNames!)
            {
                stringBuilderArgs.Append($@"
            request.{argName},");
            }

            return string.Format(GrpcTemplates.GrpcMethodTemplate, methodName, serviceName, grpcMethodName, stringBuilderArgs, _receiver.GrpcMethodResponceInfo!.FieldName);
        }

        /// <summary>
        /// Генерация строки, содержащей код стримингово метода
        /// </summary>
        private string GenerateStreamMethods(string serviceName, string methodName, string grpcMethodName, IEnumerable<AttributeData> grpcStreamAttrs, Compilation compilation)
        {
            var stringBuilderArgs  = new StringBuilder();
            var stringBuilderCases = new StringBuilder();

            for (int i = 0; i < grpcStreamAttrs.Count(); i++)
            {
                var namedArgs = grpcStreamAttrs.ElementAt(i).NamedArguments;
                string notificationIteration = $"notification{i + 1}";

                if(namedArgs.Count() == 2)
                {
                    var stringBuilderNotifArgs = new StringBuilder();

                    string classNotificationName = "";
                    string classMessageName = "";

                    foreach(var namedArg in namedArgs)
                    {
                        switch (namedArg.Key)
                        {
                            case Config.GRPC_NOTIFICATION_PARAM_NAME:
                                classNotificationName = namedArg.Value.GetShortName()!;
                            break;

                            case Config.GRPC_MESSAGE_PARAM_NAME:
                                classMessageName = namedArg.Value.GetShortName()!;
                            break;
                        }
                    }

                    foreach (var syntaxTree in compilation.SyntaxTrees)
                    {
                        var propertiesName = new List<string>();
                        //Поиск реализации класса/интерфейса
                        var notificationClassSyntax = syntaxTree.GetRoot()
                                                     .DescendantNodesAndSelf()
                                                     .OfType<ClassDeclarationSyntax>()
                                                     .FirstOrDefault(x => x.Identifier.ToString() == classNotificationName);

                        if (notificationClassSyntax == null)
                            continue;

                        string notificationName = notificationClassSyntax.Identifier.ToString();

                        //Добавление имени класса и имена всех его свойств, если такой класс ещё не был добавлен
                        if (_grpcClassAndProperties.ContainsKey(notificationClassSyntax.Identifier.ToString()))
                        {
                            propertiesName = _grpcClassAndProperties[notificationClassSyntax.Identifier.ToString()];
                        }
                        else
                        {
                            var semanticModel = compilation.GetSemanticModel(notificationClassSyntax.SyntaxTree);
                            var propertiesSyntax = notificationClassSyntax.DescendantNodesAndSelf().OfType<PropertyDeclarationSyntax>();

                            foreach(var fieldSyntax in propertiesSyntax)
                            {
                                propertiesName.Add(fieldSyntax.Identifier.Text);
                            }

                            _grpcClassAndProperties.Add(notificationName, propertiesName);
                        }

                        if (propertiesName.Count == 0)
                            continue;

                        foreach(var fieldName in propertiesName)
                        {
                            stringBuilderNotifArgs.Append($@"
                                                {notificationIteration}.{fieldName},");
                        }

                        stringBuilderNotifArgs.Length -= 1; //Удаление последней запятой
                        stringBuilderCases.AppendFormat(GrpcTemplates.GrpcCaseTemplate, 
                                                        namedArgs[0].Value.GetShortName(), 
                                                        notificationIteration, 
                                                        $@"new {classMessageName}({stringBuilderNotifArgs}
                                               )");
                    }
                }
            }

            foreach (var argName in _receiver!.GrpcMethodRequestInfo!.FieldNames!)
            {
                stringBuilderArgs.Append($@"
              request.{argName},");
            }

            return string.Format(GrpcTemplates.GrpcStreamMethodTemplate, methodName, serviceName, grpcMethodName, stringBuilderArgs, stringBuilderCases);
        }

        /// <summary>
        /// Проверка, реализует ли класс-шаблон все методы Grpc-сервиса
        /// </summary>
        private void CheckValidImplementation(IEnumerable<MethodDeclarationSyntax> methodNodes)
        {
            if(!methodNodes.Any(x => _receiver!.GrpcBaseClassInfo!.GrpcBaseMethods!.Contains(x.Identifier.ToString())))
            {
                throw new GrpcMethodCountException(string.Join(", ", _receiver!.GrpcBaseClassInfo!.GrpcBaseMethods));
            }
        }

        /// <summary>
        /// Генерация строки, содержащая в себе код для включения всех используемых пространств имён в класс
        /// </summary>
        /// <param name="serviceNamespaces">Имена используемых пространств имён в классе</param>
        private string GenerateUsings(IEnumerable<string> serviceNamespaces)
        {
            var stringBuilderUsings  = new StringBuilder();

            foreach(var serviceNamespace in serviceNamespaces)
            {
                stringBuilderUsings.AppendFormat(GrpcTemplates.UsingTemplate, serviceNamespace);
            }

            return stringBuilderUsings.ToString();
        }
    }
}

