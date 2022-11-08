using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Services;

public class SimpleService : ISimpleService
{
    public int FieldVal = 0;
    public string? FieldRef;
    public object? FieldObj;


    public int PropVal { get; set; } = 0;
    public string? PropRef { get; set; }
    public object? PropObj { get; set; }


    public Task<object?> AsyncCastMethod(object? a, int b = 222)
    {
        throw new NotImplementedException();
    }

    public Task<int> AsyncMethod(int a, int b)
    {
        throw new NotImplementedException();
    }

    public Task AsyncVoidMethod(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public object MethodObj(object a)
    {
        throw new NotImplementedException();
    }

    public string MethodRef(string a)
    {
        throw new NotImplementedException();
    }

    public Stream MethodStream(Stream a)
    {
        throw new NotImplementedException();
    }

    public int MethodVal(int a, int? b)
    {
        throw new NotImplementedException();
    }

    public void MethodVoid()
    {
        throw new NotImplementedException();
    }
}
