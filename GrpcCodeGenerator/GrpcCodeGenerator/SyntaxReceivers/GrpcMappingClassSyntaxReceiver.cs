using GrpcCodeGenerator.Configs;
using GrpcCodeGenerator.Extentions;
using GrpcCodeGenerator.InfoModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GrpcCodeGenerator.SyntaxReceivers
{
    /// <summary>
    /// Ресивер, получающий заранее известную информацию
    /// </summary>
    internal class GrpcMappingClassSyntaxReceiver : ISyntaxContextReceiver
    {
        private readonly string _grpcBaseClassShortName;

        public GrpcBaseClassInfo?       GrpcBaseClassInfo      { get; private set; }
        public GrpcMethodRequestInfo?   GrpcMethodRequestInfo  { get; private set; }
        public GrpcMethodResponceInfo?  GrpcMethodResponceInfo { get; private set; }

        public List<ClassDeclarationSyntax> MappingClasses { get; private set; } = new List<ClassDeclarationSyntax>();

        public GrpcMappingClassSyntaxReceiver()
        {
            _grpcBaseClassShortName = $"{Config.GRPC_BASE_CLASS_NAME.GetShortName()}Base";
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var semanticModel = context.SemanticModel;

            //Проверка, что просматриваемая нода является классом
            if(context.Node is ClassDeclarationSyntax classSyntaxes)
            {
                //Проверка, является ли данный класс базовым Grpc сервисом
                if (classSyntaxes.Identifier.Text == _grpcBaseClassShortName)
                {
                    GrpcBaseClassInfo = new GrpcBaseClassInfo(classSyntaxes, semanticModel);
                }

                //Проверка, является ли данный класс Grpc-сообщением типа "MethodRequest"
                if (classSyntaxes.Identifier.Text == Config.GRPC_REQUEST_NAME)
                {
                    GrpcMethodRequestInfo = GetMethodRequestInfo(classSyntaxes, semanticModel);
                }


                //Проверка, является ли данный класс Grpc-сообщением типа "MethodResponse"
                if (classSyntaxes.Identifier.Text == Config.GRPC_RESPONCE_NAME)
                {
                    GrpcMethodResponceInfo = GetGrpcMethodResponceInfo(classSyntaxes, semanticModel);
                }

                if (IsValidClass(classSyntaxes, semanticModel))
                {
                    MappingClasses.Add(classSyntaxes);
                }
            }
        }

        /// <summary>
        /// Метод с инструкциями для валидации объекта ClassDeclarationSyntax 
        /// </summary>
        /// <param name="classDeclarationSyntax">Валидируемый объект</param>
        /// <param name="semanticModel">Семантическая модель, используемая для получения информации об атрибутах класса</param>
        /// <returns>Результат валидации</returns>
        private bool IsValidClass(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
        {
            if (classDeclarationSyntax == null) 
            { 
                return false;
            }

            //В идеале сюда нужно ещё добавить проверку на соответствие пространств имён (из-за того, что классы Partial) между сгенерированым классом и классом "шаблоном",
            //который наследуется от него.
            //Но в задаче такого условия нет, так что будем считать, что это условие выполняется по умолчанию 

            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            var attributes  = classSymbol?.GetAttributes();

            //Содержит ли класс атрибут
            if(!attributes.HasValue || !attributes.Value.Any(x => x.AttributeClass?.Name == Config.GRPC_ATTR_MAPPING_NAME))
            {
                return false;
            }

            //Является ли класс partial 
            if (!classDeclarationSyntax.Modifiers.Any(x => x.Text == Config.GRPC_CLASS_MODIFIER))
            {
                return false;
            }

            //Наследуется ли класс от базового Grpc класса 
            if (classSymbol!.BaseType!.Name != _grpcBaseClassShortName)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получение модели, содержащей в себе информацию о Grpc-сообщение типа MethodRequest
        /// </summary>
        private GrpcMethodRequestInfo GetMethodRequestInfo(ClassDeclarationSyntax requestMethodClassSyntax, SemanticModel semanticModel)
        {
            var fieldNames = new List<string>();

            var grpcFields = requestMethodClassSyntax.DescendantNodesAndSelf().OfType<FieldDeclarationSyntax>();

            int argsCount = grpcFields.Count((x) => 
            {
                if (x.GetText().ToString().Contains(Config.GRPC_METHOD_ARG_PART_NAME))
                {
                    fieldNames.Add(GetMethodName(x, semanticModel, Config.GRPC_METHOD_ARG_PART_NAME));
                }

                return false;
            });

            return new GrpcMethodRequestInfo() { MethodsCount = argsCount, FieldNames = fieldNames };
        }

        /// <summary>
        /// Получение модели, содержащей в себе информацию о Grpc-сообщение типа MethodResponce
        /// </summary>
        private GrpcMethodResponceInfo GetGrpcMethodResponceInfo(ClassDeclarationSyntax requestMethodClassSyntax, SemanticModel semanticModel)
        {
            var grpcFields = requestMethodClassSyntax.DescendantNodesAndSelf().OfType<FieldDeclarationSyntax>();
            var grpcField = grpcFields.First(x => x.GetText().ToString().Contains(Config.GRPC_METHOD_ARG_PART_NAME));

            return new GrpcMethodResponceInfo() { FieldName = GetMethodName(grpcField, semanticModel, Config.GRPC_METHOD_ARG_PART_NAME) };
        }

        /// <summary>
        /// Получение имени свойства из сгенерированного Grpc поля.
        /// </summary>
        /// <param name="stringForSplit">Строка, служащая для отделение имени свойства от поля</param>
        /// <returns>Имя свойства</returns>
        private string GetMethodName(FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel semanticModel, string stringForSplit)
        {
            var fieldSymbol = semanticModel?.GetDeclaredSymbol(fieldDeclarationSyntax.Declaration.Variables.First()) as IFieldSymbol;

            return fieldSymbol!.Name.Split(new[] { stringForSplit }, StringSplitOptions.None)[0];
        }
    }
}
