using MetaFile;

namespace Example.WebApi.Services;

public class ExampleService : IExampleService
{
    public int SimpleProp { get; set; } = 777;

    public int SimpleMethod(int a, int b = 10) => a * b;

    public Task<T> GenericMethod<T>(T a, CancellationToken cancellationToken) => Task.FromResult(a);

    public async Task<string> UploadStreamMethod(Stream stream, string? name = null, CancellationToken cancellationToken = default)
    {
        var filePath = Path.GetFullPath(name ?? DefaultFilename);
        using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream, cancellationToken);
        return filePath;
    }

    public Task<Stream> DownloadStreamMethod(string? name)
    {
        return Task.FromResult(File.OpenRead(Path.GetFullPath(name ?? DefaultFilename)) as Stream);
    }

    public async Task<IStreamFile> DownloadFileMethod(string? name)
    {
        name ??= DefaultFilename;

        return await Task.FromResult(new HttpFile()
        {
            Content = File.OpenRead(Path.GetFullPath(name)),
            Name = Path.GetFileName(name),
            Type = name == DefaultFilename ? "text/plain" : null,
            InlineDisposition = true,
        });
    }

    const string DefaultFilename = "example.file";

}

