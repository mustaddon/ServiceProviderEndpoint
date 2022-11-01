using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public interface ISimpleService
{
    int TestProp { get; }
    int TestMethod(int a, int b);
    Task<int> AsyncMethod(int a, int b);
    Task<object?> AsyncCastMethod(object? a, int b = 222);
    Task AsyncVoidMethod(CancellationToken cancellationToken);
}
