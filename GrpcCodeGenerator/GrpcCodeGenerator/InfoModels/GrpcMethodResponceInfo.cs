
namespace GrpcCodeGenerator.InfoModels
{
    /// <summary>
    /// Класс, содержащий в себе информацию о Grpc-сообщение типа MethodResponce
    /// </summary>
    internal class GrpcMethodResponceInfo
    {
        /// <summary>
        /// Имя поля (судя по примеру сгенерированного кода в задании, это Grpc-сообщение всегда будет содержать одно поле)
        /// </summary>
        public string? FieldName { get; set; } 
    }
}
