using MetaFile;

namespace Example.WebApi.Services;

public class SecureExampleService : IExampleService
{
    public SecureExampleService(IExampleService service, IHttpContextAccessor httpContextAccessor)
    {
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    readonly IExampleService _service;
    readonly IHttpContextAccessor _httpContextAccessor;

    public int SimpleProp
    {
        get => _service.SimpleProp;
        set => _service.SimpleProp = value;
    }

    public Task<IStreamFile> DownloadFileMethod(string? name = null)
        => _service.DownloadFileMethod(name);

    public Task<Stream> DownloadStreamMethod(string? name = null)
        => _service.DownloadStreamMethod(name);

    public Task<T> GenericMethod<T>(T a, CancellationToken cancellationToken = default)
        => _service.GenericMethod(a, cancellationToken);

    public int SimpleMethod(int a, int b = 10)
        => _service.SimpleMethod(a, b);

    public Task<string> UploadStreamMethod(Stream stream, string? name = null, CancellationToken cancellationToken = default)
        => _service.UploadStreamMethod(stream, name, cancellationToken);
}

