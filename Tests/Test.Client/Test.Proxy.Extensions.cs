#if NET5_0_OR_GREATER
using Test.Services;

namespace Test.Client;

public partial class Test
{
    [Test]
    public void TestProxyMethodValExt()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rndA = _rnd.Next();
        var rndB = _rnd.Next();
        var rndC = _rnd.Next();

        var val0 = proxy.MethodValExt(rndA, rndB, rndC);

        Assert.That(val0, Is.EqualTo(rndA + rndB + rndC));
    }

    [Test]
    public void TestProxyMethodRefExt()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rndA = $"str_{_rnd.Next()}";
        var rndB = _rnd.Next();

        // B as int
        var val0 = proxy.MethodRefExt(rndA, rndB);

        Assert.That(val0, Is.EqualTo(rndA + rndB));

        // B as string
        var val1 = proxy.MethodRefExt(rndA, rndB.ToString());

        Assert.That(val1, Is.EqualTo(rndA + rndB));

        // B as null
        var val2 = proxy.MethodRefExt(rndA, (int?)null);

        Assert.That(val2, Is.EqualTo(rndA));
    }

    [Test]
    public void TestProxyGenericMethodExt()
    {
        var proxy = _client.GetService<IGenericService<object>>();
        var rndA = _rnd.Next();

        // B as int
        var val0 = proxy.MethodExt(rndA);

        Assert.That(val0, Is.EqualTo(rndA));

        // B as string
        var val1 = proxy.MethodExt(rndA.ToString());

        Assert.That(val1, Is.EqualTo(rndA.ToString()));

        // B as null
        var val2 = proxy.MethodExt(null);

        Assert.That(val2, Is.Null);
    }

    [Test]
    public void TestProxyGenericMethodExtInt()
    {
        var proxy = _client.GetService<IGenericService<int>>();
        var rndA = _rnd.Next();
        var rndB = _rnd.Next();

        var val0 = proxy.MethodExtInt(rndA, rndB);

        Assert.That(val0, Is.EqualTo(rndA + rndB));
    }

}
#endif