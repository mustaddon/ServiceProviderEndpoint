#if NET5_0_OR_GREATER
using System.Collections.Generic;

namespace Test.Client;

public partial class Test
{

    [Test]
    public void TestProxyMethodVoid()
    {
        var proxy = _client.GetService<ISimpleService>();

        var val0 = proxy.PropVal;

        proxy.MethodVoid();

        var val1 = proxy.PropVal;

        Assert.That(val0, Is.EqualTo(val1 - 1));
    }

    [Test]
    public void TestProxyMethodVal()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rnd = _rnd.Next();

        var val0 = proxy.MethodVal(rnd, rnd * 2);

        Assert.That(val0, Is.EqualTo(rnd * 3));
    }

    [Test]
    public void TestProxyMethodType()
    {
        var proxy = _client.GetService<ISimpleService>();

        // int
        var type = typeof(int);
        var res = proxy.MethodType(type);
        Assert.That(res, Is.EqualTo(type));

        // string
        type = typeof(string);
        res = proxy.MethodType(type);
        Assert.That(res, Is.EqualTo(type));

        // array
        type = typeof(int?[]);
        res = proxy.MethodType(type);
        Assert.That(res, Is.EqualTo(type));

        // generic
        type = typeof(List<string>);
        res = proxy.MethodType(type);
        Assert.That(res, Is.EqualTo(type));

        // generic open
        type = typeof(List<>);
        res = proxy.MethodType(type);
        Assert.That(res, Is.EqualTo(type));

        // null
        res = proxy.MethodType(null);
        Assert.That(res, Is.Null);
    }

    [Test]
    public async Task TestProxyMethodStream()
    {
        var proxy = _client.GetService<ISimpleService>();

        using var rnd = _rnd.NextStream(out var text);

        using var val0 = proxy.MethodStream(rnd);

        Assert.That(await val0?.ToText()!, Is.EqualTo(text));
    }

    [Test]
    public async Task TestProxyMethodStreamExtras()
    {
        var proxy = _client.GetService<ISimpleService>();

        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = proxy.MethodStreamExtras(rnd1.Content, rnd1.Name!);

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
    }

    [Test]
    public async Task TestProxyMethodFileStream()
    {
        var proxy = _client.GetService<ISimpleService>();

        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = proxy.MethodFileStream(rnd1);

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
        Assert.That(val1?.Type, Is.EqualTo(rnd1.Type));

        // send as non typed
        using var rnd2 = _rnd.NextStreamFile(out var text2);
        rnd2.Type = null;

        using var val2 = proxy.MethodFileStream(rnd2);

        Assert.That(await val2?.Content.ToText()!, Is.EqualTo(text2));
        Assert.That(val2?.Name, Is.EqualTo(rnd2.Name));
        Assert.That(val2?.Type, Is.EqualTo("application/octet-stream"));
    }
}
#endif