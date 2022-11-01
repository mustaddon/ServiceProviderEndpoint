using System.Threading;
using System.Threading.Tasks;

namespace Example;

public interface IExampleGenericService<T>
{
    Task<T> SimpleMethod(T a, CancellationToken cancellationToken = default);
}
