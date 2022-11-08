using Example;
using MediatR;
using ServiceProviderEndpoint.Client;


// create client
using var client = new SpeClient("https://localhost:7149/services");
var r =
    await client.CreateRequest<IExampleService>()
    //.Member(x => x.SimpleProp, 888)
    //.Member(x => x.GenericMethod(111, CancellationToken.None))
    .Member(x => x.CastMethod("asd"))
    //.Member(x => x.SimpleMethod(11,22))
    //.Member(x => x.VoidMethod())
    //.Member(x => x.AsyncVoidMethod())
    //.Parameters(typeof(int?))
    //.ReturnType(typeof(int?))
    .Send();

// ping request
var result = await client.GetService<IMediator>().Send(new Ping { Message = "TEST" });
Console.WriteLine($"result: {result.Message}");


Console.WriteLine("done");