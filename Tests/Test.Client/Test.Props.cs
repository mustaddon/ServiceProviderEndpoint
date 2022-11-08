namespace Test.Client;

public partial class Test
{

    [Test]
    public async Task TestPropVal()
    {
        var rnd = _rnd.Next();

        // get
        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropVal)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropVal, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropVal)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public async Task TestPropRef()
    {
        var rnd = $"str_{_rnd.Next()}";

        // get
        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropRef)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropRef, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropRef)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        // set null
        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropRef, null)
            .Send();

        // get
        var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropRef)
            .Send();

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestPropObj()
    {
        object rnd = _rnd.Next();

        // get
        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set val
        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // set ref
        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj, rnd)
            .Send();

        // get
        var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj)
            .Send();

        Assert.That(val2, Is.EqualTo(rnd));

        // set null
        await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj, null)
            .Send();

        // get
        var val3 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.PropObj)
            .Send();

        Assert.That(val3, Is.Null);
    }


}
