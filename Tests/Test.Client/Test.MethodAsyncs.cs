using System.Collections.Generic;
using System.Threading;

namespace Test.Client;

public partial class Test
{

    [Test]
    public async Task TestMethodVoidAsync()
    {
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal)
            .Send();

        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodVoidAsync(CancellationToken.None))
            .Send();

        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal)
            .Send();

        Assert.That(val0, Is.EqualTo(val1 - 1));
    }

    [Test]
    public async Task TestMethodValAsync()
    {
        var rndA = _rnd.Next(99,999);
        var rndB = _rnd.Next(1, 10);

        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodValAsync(rndA, rndB, CancellationToken.None))
            .Send();

        Assert.That(val0, Is.EqualTo(rndA + rndB));
    }

    [Test]
    public async Task TestMethodRefAsync()
    {
        var rnd = $"str_{_rnd.Next()}";

        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRefAsync(rnd, CancellationToken.None))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRefAsync(null, CancellationToken.None))
            .Send();

        Assert.That(val1, Is.Null);
    }

    [Test]
    public async Task TestMethodObjAsync()
    {
        object rnd = _rnd.Next();

        // as int
        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodObjAsync(rnd, CancellationToken.None))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // as str
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodObjAsync(rnd, CancellationToken.None))
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        // as null
        var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodObjAsync(null, CancellationToken.None))
            .Send();

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestMethodTypeAsync()
    {
        // int
        var type = typeof(int);
        var res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodTypeAsync(type, CancellationToken.None))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // string
        type = typeof(string);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodTypeAsync(type, CancellationToken.None))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // array
        type = typeof(int?[]);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodTypeAsync(type, CancellationToken.None))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // generic
        type = typeof(List<string>);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodTypeAsync(type, CancellationToken.None))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // generic open
        type = typeof(List<>);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodTypeAsync(type, CancellationToken.None))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // null
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodTypeAsync(null, CancellationToken.None))
            .Send();
        Assert.That(res, Is.Null);
    }

    [Test]
    public async Task TestMethodStreamAsync()
    {
        using var rnd = _rnd.NextStream(out var text);

        using var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodStreamAsync(rnd, CancellationToken.None))
            .Send();

        Assert.That(await val0?.ToText()!, Is.EqualTo(text));
    }

    [Test]
    public async Task TestMethodFileStreamAsync()
    {
        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodFileStreamAsync(rnd1, CancellationToken.None))
            .Send();

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
        Assert.That(val1?.Type, Is.EqualTo(rnd1.Type));

        // send as non typed
        using var rnd2 = _rnd.NextStreamFile(out var text2);
        rnd2.Type = null;

        using var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodFileStreamAsync(rnd2, CancellationToken.None))
            .Send();

        Assert.That(await val2?.Content.ToText()!, Is.EqualTo(text2));
        Assert.That(val2?.Name, Is.EqualTo(rnd2.Name));
        Assert.That(val2?.Type, Is.EqualTo("application/octet-stream"));
    }
}
