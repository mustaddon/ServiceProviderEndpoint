#if NET5_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

namespace Test.Client;

public partial class Test
{

    [Test]
    public async Task TestProxyMethodVoidAsync()
    {
        var proxy = _client.GetService<ISimpleService>();

        var val0 = proxy.PropVal;

        await proxy.MethodVoidAsync(CancellationToken.None);

        var val1 = proxy.PropVal;

        Assert.That(val0, Is.EqualTo(val1 - 1));
    }

    [Test]
    public async Task TestProxyMethodValAsync()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rndA = _rnd.Next(99,999);
        var rndB = _rnd.Next(1, 10);

        var val0 = await proxy.MethodValAsync(rndA, rndB, CancellationToken.None);

        Assert.That(val0, Is.EqualTo(rndA + rndB));
    }

    [Test]
    public async Task TestProxyMethodRefAsync()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rnd = $"str_{_rnd.Next()}";

        var val0 = await proxy.MethodRefAsync(rnd, CancellationToken.None);

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = await proxy.MethodRefAsync(null, CancellationToken.None);

        Assert.That(val1, Is.Null);
    }

    [Test]
    public async Task TestProxyMethodObjAsync()
    {
        var proxy = _client.GetService<ISimpleService>();
        object rnd = _rnd.Next();

        // as int
        var val0 = await proxy.MethodObjAsync(rnd, CancellationToken.None);

        Assert.That(val0, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // as str
        var val1 = await proxy.MethodObjAsync(rnd, CancellationToken.None);

        Assert.That(val1, Is.EqualTo(rnd));

        // as null
        var val2 = await proxy.MethodObjAsync(null, CancellationToken.None);

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestProxyMethodStreamAsync()
    {
        var proxy = _client.GetService<ISimpleService>();

        using var rnd = _rnd.NextStream(out var text);

        using var val0 = await proxy.MethodStreamAsync(rnd, CancellationToken.None);

        Assert.That(await val0?.ToText()!, Is.EqualTo(text));
    }

    [Test]
    public async Task TestProxyMethodStreamExtrasAsync()
    {
        var proxy = _client.GetService<ISimpleService>();

        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = await proxy.MethodStreamExtrasAsync(rnd1.Content, rnd1.Name!, CancellationToken.None);

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
    }

    [Test]
    public async Task TestProxyMethodFileStreamAsync()
    {
        var proxy = _client.GetService<ISimpleService>();

        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = await proxy.MethodFileStreamAsync(rnd1, CancellationToken.None);

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
        Assert.That(val1?.Type, Is.EqualTo(rnd1.Type));

        // send as non typed
        using var rnd2 = _rnd.NextStreamFile(out var text2);
        rnd2.Type = null;

        using var val2 = await proxy.MethodFileStreamAsync(rnd2, CancellationToken.None);

        Assert.That(await val2?.Content.ToText()!, Is.EqualTo(text2));
        Assert.That(val2?.Name, Is.EqualTo(rnd2.Name));
        Assert.That(val2?.Type, Is.EqualTo("application/octet-stream"));
    }
}
#endif