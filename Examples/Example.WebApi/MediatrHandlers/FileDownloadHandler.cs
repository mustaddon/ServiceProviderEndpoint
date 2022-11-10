using MediatR;
using MetaFile;

namespace Example.MediatrHandlers;

public class FileDownloadHandler : IRequestHandler<FileDownload, HttpFile>
{
    public Task<HttpFile> Handle(FileDownload request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpFile()
        {
            Content = File.OpenRead(Path.GetFullPath(request.Path!)),
            Name = Path.GetFileName(request.Path),
        });
    }
}

