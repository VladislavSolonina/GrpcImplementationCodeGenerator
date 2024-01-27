using GrpcCodeGenerator_TestProject.Interfaces;

namespace GrpcCodeGenerator_TestProject.Notifications
{
    public class CommandNotification1 : ICommandNotification
    {
        public string? Arg1 { get; set; }
    }
}
