
namespace GrpcCodeGenerator_TestProject.Attributes
{
    /// <summary>
    /// Атрибут пометки методов, в которых следует сопоставлять данные
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class GrpcStreamMappingAttribute : Attribute
    {
        public Type? Message { get; set; }
        public Type? Notification { get; set; }

        public GrpcStreamMappingAttribute() 
        { 
        }
    }
}
