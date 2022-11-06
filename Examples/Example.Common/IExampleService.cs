using SingleApi;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Example;

public interface IExampleService
{
    int SimpleProp { get; }
    int SimpleMethod(int a, int b = 10);
    Task<object> CastMethod(object a);
    Task<T> GenericMethod<T>(T a, CancellationToken cancellationToken = default);
    Task<string> UploadStreamMethod(Stream stream, string? name = null, CancellationToken cancellationToken = default);
    Task<Stream> DownloadStreamMethod(string? name = null);
    Task<SapiFile> DownloadFileMethod(string? name = null);
}
