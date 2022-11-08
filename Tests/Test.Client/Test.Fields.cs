



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



}
