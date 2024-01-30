using Grpc.Core;

using GrpcCodeGenerator_TestProject.Interfaces;
using GrpcCodeGenerator_TestProject.Notifications;
using GrpcCodeGenerator_TestProject.TestClasses;

namespace GrpcCodeGenerator_TestProject.TestClasses;

partial class ClassForMapping
{
    private readonly ImplInterface_First _serviceImplInterface_First;
    private readonly ImplInterface_Second _serviceImplInterface_Second;

    public ClassForMapping(ImplInterface_First serviceImplInterface_First, ImplInterface_Second serviceImplInterface_Second)
    {
        _serviceImplInterface_First = serviceImplInterface_First;
        _serviceImplInterface_Second = serviceImplInterface_Second;
    }

    public override partial async Task<MethodResponse> FirstMethod(MethodRequest request, ServerCallContext context)
    {
        var result = await _serviceImplInterface_First.FirstMethod(
            request.Field1,
            request.Field2,
            request.Field1258,
            context.CancellationToken);
            
        return new MethodResponse()
        {
            Responce = result.Value
        };
    }

    public override partial async Task<MethodResponse> SecondMethod(MethodRequest request, ServerCallContext context)
    {
        var result = await _serviceImplInterface_Second.NoFirstMethod(
            request.Field1,
            request.Field2,
            request.Field1258,
            context.CancellationToken);
            
        return new MethodResponse()
        {
            Responce = result.Value
        };
    }

    public override partial async Task FirstStreamMethod(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context)
    {
        await foreach(var notification in _serviceImplInterface_First.FirstStreamMethod(
              request.Field1,
              request.Field2,
              request.Field1258,
              context.CancellationToken))
        {
            switch(notification)
            {

                case CommandNotification1 notification1:
                     await response.WriteAsync(new MethodResponse 
                           {
                               MethodMessageFirstValue = new MethodMessage_First(
                                                           notification1.Arg1
                                                         )
                           });
                break;

                case CommandNotification2 notification2:
                     await response.WriteAsync(new MethodResponse 
                           {
                               MethodMessageSecondValue = new MethodMessage_Second(
                                                           notification2.CN1_Arg1,
                                                           notification2.CN1_Arg2
                                                         )
                           });
                break;

                case CommandNotification3 notification3:
                     await response.WriteAsync(new MethodResponse 
                           {
                               MethodMessageThirdValue = new MethodMessage_Third(
                                                           notification3.CN2_Arg1,
                                                           notification3.CN2_Arg2,
                                                           notification3.CN2_Arg3
                                                         )
                           });
                break;

                default:
                    throw new InvalidCastException($"Unexpected notification type {notification.GetType().FullName}");
            }
        }
    }

    public override partial async Task SecondStreamMethod(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context)
    {
        await foreach(var notification in _serviceImplInterface_Second.NoFirstStreamMethod(
              request.Field1,
              request.Field2,
              request.Field1258,
              context.CancellationToken))
        {
            switch(notification)
            {

                case CommandNotification1 notification1:
                     await response.WriteAsync(new MethodResponse 
                           {
                               MethodMessageFirstValue = new MethodMessage_First(
                                                           notification1.Arg1
                                                         )
                           });
                break;

                case CommandNotification2 notification2:
                     await response.WriteAsync(new MethodResponse 
                           {
                               MethodMessageSecondValue = new MethodMessage_Second(
                                                           notification2.CN1_Arg1,
                                                           notification2.CN1_Arg2
                                                         )
                           });
                break;

                default:
                    throw new InvalidCastException($"Unexpected notification type {notification.GetType().FullName}");
            }
        }
    }
}