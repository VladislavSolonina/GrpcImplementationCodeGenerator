
namespace GrpcCodeGenerator.InfoModels
{
    /// <summary>
    /// Класс, содержащий в себе информацию о Grpc-сообщение типа MethodRequest
    /// </summary>
    internal class GrpcMethodRequestInfo
    {
        /// <summary>
        /// Количество методов. Нужно для проверки, все ли нужные методы реализованы в шаблоне
        /// </summary>
        public int MethodsCount { get; set; }

        /// <summary>
        /// Имена всех полей Grpc-сообщения
        /// </summary>
        public List<string>? FieldNames { get; set;} 
    }
}
