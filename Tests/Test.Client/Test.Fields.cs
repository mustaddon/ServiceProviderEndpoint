namespace Test.Client;

public partial class Test
{

    [Test]
    public async Task TestFieldVal()
    {
        var rnd = _rnd.Next();

        // get
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldVal)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public async Task TestFieldRef()
    {
        var rnd = $"str_{_rnd.Next()}";

        // get
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        // set null
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef, null)
            .Send();

        // get
        var val2 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef)
            .Send();

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestFieldObj()
    {
        object rnd = _rnd.Next();

        // get
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set val
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // set ref
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj, rnd)
            .Send();

        // get
        var val2 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj)
            .Send();

        Assert.That(val2, Is.EqualTo(rnd));

        // set null
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj, null)
            .Send();

        // get
        var val3 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldObj)
            .Send();

        Assert.That(val3, Is.Null);
    }


}
