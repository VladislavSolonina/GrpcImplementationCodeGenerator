using GrpcCodeGenerator_TestProject.Models;

namespace GrpcCodeGenerator_TestProject.Interfaces
{
    public interface ImplInterface_First
    {
        Task<InterfaceResultModel> FirstMethod(object arg1, object arg2, object arg3, CancellationToken cancellationToken);
        IAsyncEnumerable<ICommandNotification> FirstStreamMethod(object arg1, object arg2, object arg3, CancellationToken cancellationToken);
    }
}
