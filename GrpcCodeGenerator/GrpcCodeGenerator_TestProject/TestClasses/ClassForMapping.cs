using Google.Protobuf;
using Google.Protobuf.Reflection;
using Grpc.Core;
using GrpcCodeGenerator_TestProject.Attributes;
using GrpcCodeGenerator_TestProject.Interfaces;
using GrpcCodeGenerator_TestProject.Notifications;

namespace GrpcCodeGenerator_TestProject.TestClasses
{
    /// <summary>
    /// Класс-шаблон, на основе которого обязан сгенерироваться код
    /// </summary>
    [GrpcMapping]
    public partial class ClassForMapping : MyGrpcApi.MyGrpcApiBase
    {
        [GrpcInterface(typeof(ImplInterface_First))]
        public override partial Task<MethodResponse> FirstMethod(MethodRequest request, ServerCallContext context);

        [GrpcInterface(typeof(ImplInterface_Second), Method = nameof(ImplInterface_Second.NoFirstMethod))]
        public override partial Task<MethodResponse> SecondMethod(MethodRequest request, ServerCallContext context);

        [GrpcInterface(typeof(ImplInterface_First))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification1), Message = typeof(MethodMessage_First))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification2), Message = typeof(MethodMessage_Second))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification3), Message = typeof(MethodMessage_Third))]
        public override partial Task FirstStreamMethod(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context);

        [GrpcInterface(typeof(ImplInterface_Second), Method = nameof(ImplInterface_Second.NoFirstStreamMethod))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification1), Message = typeof(MethodMessage_First))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification2), Message = typeof(MethodMessage_Second))]
        public override partial Task SecondStreamMethod(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context);

        public void RandomMethod()
        {

        }
    }
}
