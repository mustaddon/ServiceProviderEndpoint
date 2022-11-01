using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public interface IGenericService<TA, TB>
{
    TA? TestProp { get; }
    TA SyncMethod(TA i);
    Task<TA> AsyncMethod(TA a, TB b);
    Task<T> AsyncGenericMethod<T>(T a, CancellationToken cancellationToken);
}
