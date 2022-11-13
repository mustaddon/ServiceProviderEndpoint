namespace Test.Client;

public partial class Test
{
    [Test]
    public async Task TestGenericPropVal()
    {
        var rnd = _rnd.Next();
        var serviceRequest = _client.CreateRequest<IGenericService<int>>();

        // get
        var val0 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await serviceRequest
            .Member(x => x.Prop, rnd)
            .Send();

        // get
        var val1 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public async Task TestGenericPropRef()
    {
        var rnd = $"str_{_rnd.Next()}";
        var serviceRequest = _client.CreateRequest<IGenericService<string>>();

        // get
        var val0 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await serviceRequest
            .Member(x => x.Prop, rnd)
            .Send();

        // get
        var val1 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        // set null
        await serviceRequest
            .Member(x => x.Prop, null)
            .Send();

        // get
        var val2 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestGenericPropObj()
    {
        object rnd = _rnd.Next();
        var serviceRequest = _client.CreateRequest<IGenericService<object>>();

        // get
        var val0 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set val
        await serviceRequest
            .Member(x => x.Prop, rnd)
            .Send();

        // get
        var val1 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // set ref
        await serviceRequest
            .Member(x => x.Prop, rnd)
            .Send();

        // get
        var val2 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val2, Is.EqualTo(rnd));

        // set null
        await serviceRequest
            .Member(x => x.Prop, null)
            .Send();

        // get
        var val3 = await serviceRequest
            .Member(x => x.Prop)
            .Send();

        Assert.That(val3, Is.Null);
    }
}
