
namespace GrpcCodeGenerator_TestProject.TestClasses
{
    /// <summary>
    /// Случайный класс, который не должен генерировать код
    /// </summary>
    public class RandomClass
    {
        public string? Name { get; set; }

        public void HelloWorld()
        {
            Console.WriteLine(Name);
        }
    }
}
