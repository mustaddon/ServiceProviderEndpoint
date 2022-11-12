using System.Collections.Generic;
using System.Linq;

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
    public async Task TestFieldType()
    {
        var type = typeof(int);

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldType, type)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldType)
            .Send();

        Assert.That(val1, Is.EqualTo(type));

        type = typeof(string);

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldType, type)
            .Send();

        // get
        var val2 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldType)
            .Send();

        Assert.That(val2, Is.EqualTo(type));

        type = typeof(int?[]);

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldType, type)
            .Send();

        // get
        var val3 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldType)
            .Send();

        Assert.That(val3, Is.EqualTo(type));

        // set null
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef, null)
            .Send();

        // get
        var val4 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldRef)
            .Send();

        Assert.That(val4, Is.Null);
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

    [Test]
    public async Task TestFieldArray()
    {
        var rnd = _rnd.NextEnumerable(x => _rnd.Next()).ToArray();

        // get
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldArray)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldArray, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldArray)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public async Task TestFieldList()
    {
        var rnd = _rnd.NextEnumerable(x => (int?)_rnd.Next()).ToList();

        // get
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldList)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldList, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldList)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));
    }

    [Test]
    public async Task TestFieldEnumerable()
    {
        var rnd = _rnd.NextEnumerable(x => _rnd.Next()).ToArray().AsEnumerable();

        // get
        var val0 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldEnumerable)
            .Send();

        Assert.That(val0, Is.Not.EqualTo(rnd));

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldEnumerable, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldEnumerable)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));


        // set null
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldEnumerable, null)
            .Send();

        // get
        var val2 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldEnumerable)
            .Send();

        Assert.That(val2, Is.Null);
    }

    [Test]
    public async Task TestFieldTypes()
    {
        var types = new[] { typeof(string), null, typeof(List<int?>) }; 
        var rnd = _rnd.NextEnumerable(x => types[_rnd.Next(0, types.Length)]).ToArray().AsEnumerable();

        // set
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldTypes, rnd)
            .Send();

        // get
        var val1 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldTypes)
            .Send();

        Assert.That(val1, Is.EqualTo(rnd));


        // set null
        await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldTypes, null)
            .Send();

        // get
        var val2 = await _client.CreateRequest<SimpleService>()
            .Member(x => x.FieldTypes)
            .Send();

        Assert.That(val2, Is.Null);
    }

}
