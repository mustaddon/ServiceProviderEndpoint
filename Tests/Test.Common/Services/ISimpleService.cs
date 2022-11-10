using MetaFile;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public interface ISimpleService
{
    int PropVal { get; set; }
    string? PropRef { get; set; }
    object? PropObj { get; set; }

    void MethodVoid();
    int MethodVal(int a, int? b);
    string? MethodRef(string? a);
    object? MethodObj(object? a);
    Type? MethodType(Type? a);
    Stream MethodStream(Stream a);
    IStreamFile MethodFileStream(IStreamFile a);

}
