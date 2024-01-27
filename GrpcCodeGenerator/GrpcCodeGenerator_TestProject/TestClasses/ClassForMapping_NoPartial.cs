using Grpc.Core;
using GrpcCodeGenerator_TestProject.Attributes;
using GrpcCodeGenerator_TestProject.Interfaces;
using GrpcCodeGenerator_TestProject.MethodMessages;
using GrpcCodeGenerator_TestProject.Notifications;

namespace GrpcCodeGenerator_TestProject.TestClasses
{
    /// <summary>
    /// Класс-шаблон, на основе которого не должен сгенерироваться код
    /// </summary>
    [GrpcMapping]
    public class ClassForMapping_NoPartial : MyGrpcApi.MyGrpcApiBase
    {
        [GrpcInterface(typeof(ImplInterface_First))]
        public override Task<MethodResponse> FirstMethod(MethodRequest request, ServerCallContext context)
        {
            return Task.FromResult(new MethodResponse());
        }

        [GrpcInterface(typeof(ImplInterface_Second), Method = nameof(ImplInterface_Second.NoFirstMethod))]
        public override Task<MethodResponse> SecondMethod(MethodRequest request, ServerCallContext context)
        {
            return Task.FromResult(new MethodResponse());
        }

        [GrpcInterface(typeof(ImplInterface_First))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification1), Message = typeof(MethodMessage_First))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification2), Message = typeof(MethodMessage_Second))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification3), Message = typeof(MethodMessage_Third))]
        public override Task FirstStreamMethod(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context)
        {
            return Task.FromResult("");
        }

        [GrpcInterface(typeof(ImplInterface_Second), Method = nameof(ImplInterface_Second.NoFirstStreamMethod))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification1), Message = typeof(MethodMessage_First))]
        [GrpcStreamMapping(Notification = typeof(CommandNotification2), Message = typeof(MethodMessage_Second))]
        public override Task SecondStreamMethod(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context)
        {
            return Task.FromResult("");
        }
    }
}
