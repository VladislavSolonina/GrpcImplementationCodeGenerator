
namespace GrpcCodeGenerator.Templates
{
    /// <summary>
    /// Класс, содержащий в себе шаблоны для генерации кода. Сюда вынесена даже мелочь только для того, чтобы код, реализующий логику генерации, не засорялся
    /// </summary>
    internal static class GrpcTemplates
    {
        public readonly static string GrpcClassTemplate = @"using Grpc.Core;
{0}

namespace {1};

partial class {2}
{{{3}
}}";

        public readonly static string GrpcConstructorParameterTemplate = @"{0} service{0}, ";

        public readonly static string GrpcMethodTemplate = @"

    public override partial async Task<MethodResponse> {0}(MethodRequest request, ServerCallContext context)
    {{
        var result = await _service{1}.{2}({3}
            context.CancellationToken);
            
        return new MethodResponse()
        {{
            {4} = result.Value
        }};
    }}";

        public readonly static string GrpcStreamMethodTemplate = @"

    public override partial async Task {0}(MethodRequest request, IServerStreamWriter<MethodResponse> response, ServerCallContext context)
    {{
        await foreach(var notification in _service{1}.{2}({3}
              context.CancellationToken))
        {{
            switch(notification)
            {{{4}

                default:
                    throw new InvalidCastException($""Unexpected notification type {{notification.GetType().FullName}}"");
            }}
        }}
    }}";

        public readonly static string GrpcCaseTemplate = @"

                case {0} {1}:
                     await response.WriteAsync(new MethodResponse 
                           {{
                               {2}Value = {3}
                           }});
                break;";

        public readonly static string MethodBodyTemplate = @"
    {{{0}
    }}";

        public readonly static string PublicMethodHeadTemplate = @"

    public {0}({1})";

        public readonly static string FieldsTemplate = @"
    private readonly {0} _service{0};";

        public readonly static string ServiceItemTemplate = @"
        _service{0} = service{0};";

        public readonly static string UsingTemplate = @"
using {0};";

    }
}
