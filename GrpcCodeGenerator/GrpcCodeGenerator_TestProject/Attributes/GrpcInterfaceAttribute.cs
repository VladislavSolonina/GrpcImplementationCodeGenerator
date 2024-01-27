
namespace GrpcCodeGenerator_TestProject.Attributes
{
    /// <summary>
    /// Атрибут пометки методов, на основе которых будут генерироваться методы
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GrpcInterfaceAttribute : Attribute
    {
        public string? Method { get; set; }

        public GrpcInterfaceAttribute(Type type) 
        {
        }
    }
}
