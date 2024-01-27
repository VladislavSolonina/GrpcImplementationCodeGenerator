using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GrpcCodeGenerator.InfoModels
{
    /// <summary>
    /// Класс, содержащий в себе информацию о Grpc-сервисе
    /// </summary>
    internal class GrpcBaseClassInfo
    {
        /// <summary>
        /// Отображение базового класса, реализующего Grpc-сервис, в виде синтаксиса класса
        /// </summary>
        public ClassDeclarationSyntax? GrpcBaseClass { get; set; }

        /// <summary>
        /// Имена всех методов Grpc-сервиса
        /// </summary>
        public List<string>? GrpcBaseMethods { get; set; }

        /// <summary>
        /// Пространство имён базового класса, реализующего Grpc-сервис
        /// </summary>
        public string? Namespace { get; set; }

        public GrpcBaseClassInfo(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
        {
            GrpcBaseClass   = classDeclarationSyntax;
            GrpcBaseMethods = GetMethodNames(classDeclarationSyntax);
            Namespace       = GetNamespace(classDeclarationSyntax, semanticModel);
        }

        /// <summary>
        /// Получение имён методов базового класса, реализующего Grpc-сервис
        /// </summary>
        private List<string> GetMethodNames(ClassDeclarationSyntax classDeclarationSyntax)
        {
            var methodList = new List<string>();
            var grpcBaseClassMethods = classDeclarationSyntax.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>();

            foreach (var grpcBaseClassMethod in grpcBaseClassMethods)
            {
                methodList.Add(grpcBaseClassMethod.Identifier.ToString());
            }

            return methodList;
        }

        /// <summary>
        /// Получение пространства имён базового класса, реализующего Grpc-сервис
        /// </summary>
        private string GetNamespace(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel)
        {
            return semanticModel.GetDeclaredSymbol(classDeclarationSyntax)!.ContainingNamespace.ToString();
        }

    }
}
