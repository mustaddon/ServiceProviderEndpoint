using Example;
using MediatR;
using ServiceProviderEndpoint.Client;


// create client
using var client = new SpeClient("https://localhost:7149/services");

// ping request
var result = await client.GetService<IMediator>().Send(new Ping { Message = "TEST" });

Console.WriteLine($"result: {result.Message}");

Console.WriteLine("done");