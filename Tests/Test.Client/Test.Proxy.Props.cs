#if NET5_0_OR_GREATER
using System.Linq;

namespace Test.Client;

public partial class Test
{

    [Test]
    public void TestProxyPropVal()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rnd = _rnd.Next();

        // get
        var val0 = proxy.PropVal;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        proxy.PropVal = rnd;

        // get
        var val1 = proxy.PropVal;

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public void TestProxyPropRef()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rnd = $"str_{_rnd.Next()}";

        // get
        var val0 = proxy.PropRef;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        proxy.PropRef = rnd;

        // get
        var val1 = proxy.PropRef;

        Assert.That(val1, Is.EqualTo(rnd));

        // set null
        proxy.PropRef = null;

        // get
        var val2 = proxy.PropRef;

        Assert.That(val2, Is.Null);
    }

    [Test]
    public void TestProxyPropArray()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rnd = _rnd.NextEnumerable(x => _rnd.Next()).ToArray();

        // get
        var val0 = proxy.PropArray;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        proxy.PropArray = rnd;

        // get
        var val1 = proxy.PropArray;

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public void TestProxyPropEnumerable()
    {
        var proxy = _client.GetService<ISimpleService>();
        var rnd = _rnd.NextEnumerable(x => _rnd.Next()).ToArray().AsEnumerable();

        // get
        var val0 = proxy.PropEnumerable;

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        proxy.PropEnumerable = rnd;

        // get
        var val1 = proxy.PropEnumerable;

        Assert.That(val1, Is.EqualTo(rnd));
    }


}
#endif