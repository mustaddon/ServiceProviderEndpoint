# ServiceProviderEndpoint client [![NuGet version](https://badge.fury.io/nu/ServiceProviderEndpoint.Client.svg?)](http://badge.fury.io/nu/ServiceProviderEndpoint.Client)


## Example: Usage
```C#
using ServiceProviderEndpoint.Client;

// create client
using var client = new SpeClient("https://localhost:7149/services");

// send request
var result = await client.GetService<IMediator>().Send(new Ping { Message = "TEST" });

Console.WriteLine(result?.Message);
```

*Console output:*
```
TEST PONG
```

[Example project...](https://github.com/mustaddon/ServiceProviderEndpoint/tree/main/Examples/Example.Client)
