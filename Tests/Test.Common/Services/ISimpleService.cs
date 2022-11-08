using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public interface ISimpleService
{
    int PropVal { get; set; }
    string? PropRef { get; set; }
    object? PropObj { get; set; }

    void MethodVoid();
    int MethodVal(int a, int? b);
    string MethodRef(string a);
    object MethodObj(object a);
    Stream MethodStream(Stream a);


    Task<int> AsyncMethod(int a, int b);
    Task<object?> AsyncCastMethod(object? a, int b = 222);
    Task AsyncVoidMethod(CancellationToken cancellationToken);
}
