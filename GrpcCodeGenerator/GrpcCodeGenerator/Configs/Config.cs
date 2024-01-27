
namespace GrpcCodeGenerator.Configs
{
    /// <summary>
    /// Класс, содержащий в себе некоторые конфигурации. В данном случае сюда вынесена заранее известная информация
    /// </summary>
    internal class Config
    {
        public const string GRPC_ATTR_MAPPING_NAME        = "GrpcMappingAttribute";
        public const string GRPC_ATTR_INTERFACE_NAME      = "GrpcInterfaceAttribute";
        public const string GRPC_ATTR_STREAM_MAPPING_NAME = "GrpcStreamMappingAttribute";

        public const string GRPC_BASE_CLASS_NAME         = "MyGrpcApi";
        public const string GRPC_REQUEST_NAME            = "MethodRequest";
        public const string GRPC_RESPONCE_NAME           = "MethodResponse";
        public const string GRPC_METHOD_ARG_PART_NAME    = "FieldNumber";
        public const string GRPC_NOTIFICATION_PARAM_NAME = "Notification";
        public const string GRPC_MESSAGE_PARAM_NAME      = "Message";

        public const string GRPC_CLASS_MODIFIER      = "partial";
    }
}
