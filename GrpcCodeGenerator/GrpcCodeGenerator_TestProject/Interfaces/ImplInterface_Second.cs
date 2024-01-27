using GrpcCodeGenerator_TestProject.Models;

namespace GrpcCodeGenerator_TestProject.Interfaces
{
    public interface ImplInterface_Second
    {
        Task<InterfaceResultModel> NoFirstMethod(object arg1, object arg2, object arg3, CancellationToken cancellationToken);
        IAsyncEnumerable<ICommandNotification> NoFirstStreamMethod(object arg1, object arg2, object arg3, CancellationToken cancellationToken);
    }
}
