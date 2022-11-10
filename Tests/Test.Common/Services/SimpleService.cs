using MetaFile;
using System;
using System.IO;

namespace Test.Services;

public class SimpleService : ISimpleService
{
    public int FieldVal = 0;
    public string? FieldRef;
    public object? FieldObj;


    public int PropVal { get; set; } = 0;
    public string? PropRef { get; set; }
    public object? PropObj { get; set; }


    public void MethodVoid() => FieldVal++;
    public int MethodVal(int a, int? b) => a + (b ?? 0);
    public string? MethodRef(string? a) => a;
    public object? MethodObj(object? a) => a;
    public Type? MethodType(Type? a) => a;


    public Stream MethodStream(Stream a)
    {
        var ms = new MemoryStream();
        a.CopyToAsync(ms).Wait();
        ms.Position = 0;
        return ms;
    }

    public IStreamFile MethodFileStream(IStreamFile a)
    {
        var ms = new MemoryStream();
        a.Content.CopyToAsync(ms).Wait();
        ms.Position = 0;

        return new StreamFile
        {
            Content = ms,
            Name = a.Name,
            Type = a.Type,
        };
    }
}
