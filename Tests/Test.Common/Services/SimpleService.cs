using MetaFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public class SimpleService : ISimpleService
{
    public int FieldVal = 0;
    public string? FieldRef;
    public Type? FieldType;
    public object? FieldObj;
    public int[] FieldArray = Array.Empty<int>();
    public List<int?> FieldList = new();
    public IEnumerable<int> FieldEnumerable = Array.Empty<int>();
    public IEnumerable<Type?> FieldTypes = new[] { typeof(string), null };


    public int PropVal { get; set; } = 0;
    public string? PropRef { get; set; }
    public object? PropObj { get; set; }
    public int[] PropArray { get; set; } = Array.Empty<int>();
    public List<int?> PropList { get; set; } = new();
    public IEnumerable<int> PropEnumerable { get; set; } = Array.Empty<int>();


    public void MethodVoid() => FieldVal++;
    public int MethodVal(int a, int? b) => a + (b ?? 0);
    public string? MethodRef(string? a) => a;
    public object? MethodObj(object? a) => a;
    public Type? MethodType(Type? a) => a;
    public Stream MethodStream(Stream a) => MethodStreamAsync(a).Result;
    public IStreamFile MethodFileStream(IStreamFile a) => MethodFileStreamAsync(a).Result;


    public Task MethodVoidAsync(CancellationToken cancellationToken) => Task.Run(() => MethodVoid(), cancellationToken);
    public Task<int> MethodValAsync(int a, int? b, CancellationToken cancellationToken) => Task.FromResult(MethodVal(a, b));
    public Task<string?> MethodRefAsync(string? a, CancellationToken cancellationToken) => Task.FromResult(MethodRef(a));
    public Task<object?> MethodObjAsync(object? a, CancellationToken cancellationToken) => Task.FromResult(MethodObj(a));
    public Task<Type?> MethodTypeAsync(Type? a, CancellationToken cancellationToken) => Task.FromResult(MethodType(a));
    
    public async Task<Stream> MethodStreamAsync(Stream a, CancellationToken cancellationToken = default)
    {
        var ms = new MemoryStream();
        await a.CopyToAsync(ms);
        ms.Position = 0;
        return ms;
    }

    public async Task<IStreamFile> MethodFileStreamAsync(IStreamFile a, CancellationToken cancellationToken = default)
    {
        var ms = new MemoryStream();
        await a.Content.CopyToAsync(ms);
        ms.Position = 0;

        return new StreamFile
        {
            Content = ms,
            Name = a.Name,
            Type = a.Type,
        };
    }
}
