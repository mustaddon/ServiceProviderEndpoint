using MetaFile;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Example;

public interface IExampleService
{
    int SimpleProp { get; set; }
    int SimpleMethod(int a, int b = 10);
    Task<T> GenericMethod<T>(T a, CancellationToken cancellationToken = default);
    Task<string> UploadStreamMethod(Stream stream, string? name = null, CancellationToken cancellationToken = default);
    Task<Stream> DownloadStreamMethod(string? name = null);
    Task<IStreamFile> DownloadFileMethod(string? name = null);
}
