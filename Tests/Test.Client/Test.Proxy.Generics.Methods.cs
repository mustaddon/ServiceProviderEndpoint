#if NET5_0_OR_GREATER
namespace Test.Client;

public partial class Test
{
    [Test]
    public void TestProxyGenericMethodVal()
    {
        var proxy = _client.GetService<IGenericService<int>>();
        var rnd = _rnd.Next();

        var val0 = proxy.Method(rnd);

        Assert.That(val0, Is.EqualTo(rnd));
    }

    [Test]
    public void TestProxyGenericMethodRef()
    {
        var proxy = _client.GetService<IGenericService<string>>();
        var rnd = $"str_{_rnd.Next()}";

        var val0 = proxy.Method(rnd);

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = proxy.Method(null);

        Assert.That(val1, Is.Null);
    }

    [Test]
    public void TestProxyGenericMethodObj()
    {
        var proxy = _client.GetService<IGenericService<object>>();
        object rnd = _rnd.Next();

        var val0 = proxy.Method(rnd);

        Assert.That(val0, Is.EqualTo(rnd));

        // as null
        var val1 = proxy.Method(null);

        Assert.That(val1, Is.Null);
    }

    [Test]
    public void TestProxyGenericMethodGeneric()
    {
        var proxy = _client.GetService<IGenericService<string>>();
        var rndA = $"str_{_rnd.Next()}";
        var rndB = _rnd.Next();

        // B as int
        var val0 = proxy.MethodB(rndA, rndB);

        Assert.That(val0, Is.EqualTo(rndB));

        // B as string
        var val1 = proxy.MethodB(rndA, rndB.ToString());

        Assert.That(val1, Is.EqualTo(rndB.ToString()));

        // B as null
        var val2 = proxy.MethodB(rndA, (int?)null);

        Assert.That(val2, Is.Null);
    }

}
#endif