
namespace GrpcCodeGenerator.Exceptions
{
    /// <summary>
    /// Исключение, которое должно вызываться, если класс-шаблон не реализовал все методы Grpc-сервиса
    /// </summary>
    internal class GrpcMethodCountException : Exception
    {
        public GrpcMethodCountException(int currentCount, int expectedCount) : base(string.Format("{0} out of {1} methods implemented.", currentCount, expectedCount)) 
        { 
        }

        public GrpcMethodCountException(string expectedMethods) : base(string.Format("The following methods were expected to be implemented: \"{0}\"", expectedMethods))
        {
        }
    }
}
