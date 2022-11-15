#if NET5_0_OR_GREATER
namespace Test.Client;

public partial class Test
{
    [Test]
    public void TestProxyGenericPropVal()
    {
        var proxy = _client.GetService<IGenericService<int>>();
        var rnd = _rnd.Next();

        // get
        var val0 = proxy.Prop;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        proxy.Prop = rnd;

        // get
        var val1 = proxy.Prop;

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public void TestProxyGenericPropRef()
    {
        var proxy = _client.GetService<IGenericService<string>>();
        var rnd = $"str_{_rnd.Next()}";

        // get
        var val0 = proxy.Prop;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        proxy.Prop = rnd;

        // get
        var val1 = proxy.Prop;

        Assert.That(val1, Is.EqualTo(rnd));

        // set null
        proxy.Prop = null;

        // get
        var val2 = proxy.Prop;

        Assert.That(val2, Is.Null);
    }

    [Test]
    public void TestProxyGenericPropObj()
    {
        var proxy = _client.GetService<IGenericService<object>>();
        object rnd = _rnd.Next();

        // get
        var val0 = proxy.Prop;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set val
        proxy.Prop = rnd;

        // get
        var val1 = proxy.Prop;

        Assert.That(val1, Is.EqualTo(rnd));

        rnd = $"obj_{_rnd.Next()}";

        // set ref
        proxy.Prop = rnd;

        // get
        var val2 = proxy.Prop;

        Assert.That(val2, Is.EqualTo(rnd));

        // set null
        proxy.Prop = null;

        // get
        var val3 = proxy.Prop;

        Assert.That(val3, Is.Null);
    }
}
#endif