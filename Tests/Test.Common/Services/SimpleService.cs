using MetaFile;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public class SimpleService : ISimpleService
{
    public SimpleService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    readonly IHttpContextAccessor _httpContextAccessor;

    public string HeaderValue(string name) => _httpContextAccessor.HttpContext.Request.Headers[name];


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

    public Stream MethodStream(Stream a)
    {
        var ms = new MemoryStream();
        a.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }

    public IStreamFile MethodStreamExtras(Stream a, string name) => new StreamFile { Content = MethodStream(a), Name = name };
    public IStreamFile MethodFileStream(IStreamFile a) => new StreamFile { Content = MethodStream(a.Content), Name = a.Name, Type = a.Type };



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

    public async Task<IStreamFile> MethodStreamExtrasAsync(Stream a, string name, CancellationToken cancellationToken = default)
    {
        return new StreamFile { Content = await MethodStreamAsync(a, cancellationToken), Name = name };
    }

    public async Task<IStreamFile> MethodFileStreamAsync(IStreamFile a, CancellationToken cancellationToken = default)
    {
        return new StreamFile { Content = await MethodStreamAsync(a.Content, cancellationToken), Name = a.Name, Type = a.Type };
    }
}
