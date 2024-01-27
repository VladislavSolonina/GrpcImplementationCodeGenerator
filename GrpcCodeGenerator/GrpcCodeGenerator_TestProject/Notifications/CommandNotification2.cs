using GrpcCodeGenerator_TestProject.Interfaces;

namespace GrpcCodeGenerator_TestProject.Notifications
{
    public class CommandNotification2 : ICommandNotification
    {
        public string? CN1_Arg1 { get; set; }
        public string? CN1_Arg2 { get; set; }
    }
}
