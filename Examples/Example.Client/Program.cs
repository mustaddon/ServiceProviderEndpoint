using Example;
using MediatR;
using ServiceProviderEndpoint.Client;


// create client
using var client = new SpeClient("https://localhost:7149/services");


// send request via automatic proxy object
var result1 = await client.GetService<IMediator>()
    .Send(new Ping { Message = "TEST 1" });

Console.WriteLine(result1?.Message);


// send request via builder
var result2 = await client.CreateRequest<IMediator>()
    .Member(mediator => mediator.Send(new Ping { Message = "TEST 2" }, CancellationToken.None))
    .Send();

Console.WriteLine(result2?.Message);



Console.WriteLine("end");