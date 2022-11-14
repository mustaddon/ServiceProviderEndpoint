namespace Test.Client;

public partial class Test
{
    [Test]
    public async Task TestHeaders()
    {
        var rndName = $"name-{_rnd.Next()}";
        var rndValue = $"value_{_rnd.Next()}";

        _client.DefaultRequestHeaders.Add(rndName, rndValue);

        var val0 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.HeaderValue(rndName))
            .Send();

        var val1 = await _client.CreateRequest<ISimpleService>()
            .Member(x => x.HeaderValue(rndName))
            .Send();

        Assert.That(val0, Is.EqualTo(rndValue));
        Assert.That(val1, Is.EqualTo(rndValue));
    }

}
