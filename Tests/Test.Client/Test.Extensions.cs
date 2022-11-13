namespace Test.Client;

public partial class Test
{
    [Test]
    public async Task TestMethodValExt()
    {
        var rndA = _rnd.Next();
        var rndB = _rnd.Next();
        var rndC = _rnd.Next();

        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodValExt(rndA, rndB, rndC))
            .Send();

        Assert.That(val0, Is.EqualTo(rndA + rndB + rndC));
    }

    [Test]
    public async Task TestMethodRefExt()
    {
        var rndA = $"str_{_rnd.Next()}";
        var rndB = _rnd.Next();

        // B as int
        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRefExt(rndA, rndB))
            .Send();

        Assert.That(val0, Is.EqualTo(rndA + rndB));

        // B as string
        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRefExt(rndA, rndB.ToString()))
            .Send();

        Assert.That(val1, Is.EqualTo(rndA + rndB));

        // B as null
        var val2 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.MethodRefExt(rndA, (int?)null))
            .Send();

        Assert.That(val2, Is.EqualTo(rndA));
    }
}
