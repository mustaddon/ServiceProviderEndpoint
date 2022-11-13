namespace Test.Client;

public partial class Test
{
    [Test]
    public async Task TestGenericMethodVal()
    {
        var rnd = _rnd.Next();
        var serviceRequest = _client.CreateRequest<IGenericService<int>>();

        var val0 = await serviceRequest
            .Member(x => x.Method(rnd))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));
    }

    [Test]
    public async Task TestGenericMethodRef()
    {
        var rnd = $"str_{_rnd.Next()}";
        var serviceRequest = _client.CreateRequest<IGenericService<string>>();

        var val0 = await serviceRequest
            .Member(x => x.Method(rnd))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = await serviceRequest
            .Member(x => x.Method(null))
            .Send();

        Assert.That(val1, Is.Null);
    }

    [Test]
    public async Task TestGenericMethodObj()
    {
        object rnd = _rnd.Next();
        var serviceRequest = _client.CreateRequest<IGenericService<object>>();

        var val0 = await serviceRequest
            .Member(x => x.Method(rnd))
            .Send();

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = await serviceRequest
            .Member(x => x.Method(null))
            .Send();

        Assert.That(val1, Is.Null);
    }

    [Test]
    public async Task TestGenericMethodGeneric()
    {
        var rndA = $"str_{_rnd.Next()}";
        var rndB = _rnd.Next();
        var serviceRequest = _client.CreateRequest<IGenericService<string>>();

        // B as int
        var val0 = await serviceRequest
            .Member(x => x.MethodB(rndA, rndB))
            .Send();

        Assert.That(val0, Is.EqualTo(rndB));

        // B as string
        var val1 = await serviceRequest
            .Member(x => x.MethodB(rndA, rndB.ToString()))
            .Send();

        Assert.That(val1, Is.EqualTo(rndB.ToString()));

        // B as null
        var val2 = await serviceRequest
            .Member(x => x.MethodB(rndA, (int?)null))
            .Send();

        Assert.That(val2, Is.Null);
    }

}
