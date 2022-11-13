using MetaFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public interface ISimpleService
{
    int PropVal { get; set; }
    string? PropRef { get; set; }
    object? PropObj { get; set; }
    int[] PropArray { get; set; }
    List<int?> PropList { get; set; }
    IEnumerable<int> PropEnumerable { get; set; }

    void MethodVoid();
    int MethodVal(int a, int? b);
    string? MethodRef(string? a);
    object? MethodObj(object? a);
    Type? MethodType(Type? a);
    Stream MethodStream(Stream a);
    IStreamFile MethodFileStream(IStreamFile a);

    Task MethodVoidAsync(CancellationToken cancellationToken = default);
    Task<int> MethodValAsync(int a, int? b, CancellationToken cancellationToken);
    Task<string?> MethodRefAsync(string? a, CancellationToken cancellationToken);
    Task<object?> MethodObjAsync(object? a, CancellationToken cancellationToken);
    Task<Type?> MethodTypeAsync(Type? a, CancellationToken cancellationToken);
    Task<Stream> MethodStreamAsync(Stream a, CancellationToken cancellationToken);
    Task<IStreamFile> MethodFileStreamAsync(IStreamFile a, CancellationToken cancellationToken);

}

