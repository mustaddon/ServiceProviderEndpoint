using System.Collections.Generic;

namespace Test.Client;

public partial class Test
{

    [Test]
    public async Task TestMethodVoid()
    {
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal)
            .Send();

        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodVoid())
            .Send();

        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal)
            .Send();

        Assert.That(val0, Is.EqualTo(val1 - 1));
    }

    [Test]
    public async Task TestMethodVal()
    {
        var rnd = _rnd.Next();

        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodVal(rnd, rnd * 2))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd * 3));
    }

    [Test]
    public async Task TestMethodRef()
    {
        var rnd = $"str_{_rnd.Next()}";

        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRef(rnd))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRef(null))
            .Send();

        Assert.That(val1, Is.Null);
    }

    [Test]
    public async Task TestMethodObj()
    {
        object rnd = _rnd.Next();

        // as int
        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodObj(rnd))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // as str
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodObj(rnd))
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        // as null
        var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodObj(null))
            .Send();

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestMethodType()
    {
        // int
        var type = typeof(int);
        var res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodType(type))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // string
        type = typeof(string);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodType(type))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // array
        type = typeof(int?[]);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodType(type))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // generic
        type = typeof(List<string>);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodType(type))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // generic open
        type = typeof(List<>);
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodType(type))
            .Send();
        Assert.That(res, Is.EqualTo(type));

        // null
        res = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodType(null))
            .Send();
        Assert.That(res, Is.Null);
    }

    [Test]
    public async Task TestMethodStream()
    {
        using var rnd = _rnd.NextStream(out var text);

        using var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodStream(rnd))
            .Send();

        Assert.That(await val0?.ToText()!, Is.EqualTo(text));
    }

    [Test]
    public async Task TestMethodStreamExtras()
    {
        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodStreamExtras(rnd1.Content, rnd1.Name!))
            .Send();

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
    }

    [Test]
    public async Task TestMethodFileStream()
    {
        using var rnd1 = _rnd.NextStreamFile(out var text1);

        using var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodFileStream(rnd1))
            .Send();

        Assert.That(await val1?.Content.ToText()!, Is.EqualTo(text1));
        Assert.That(val1?.Name, Is.EqualTo(rnd1.Name));
        Assert.That(val1?.Type, Is.EqualTo(rnd1.Type));

        // send as non typed
        using var rnd2 = _rnd.NextStreamFile(out var text2);
        rnd2.Type = null;

        using var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodFileStream(rnd2))
            .Send();

        Assert.That(await val2?.Content.ToText()!, Is.EqualTo(text2));
        Assert.That(val2?.Name, Is.EqualTo(rnd2.Name));
        Assert.That(val2?.Type, Is.EqualTo("application/octet-stream"));
    }
}
