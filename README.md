# ServiceProviderEndpoint [![NuGet version](https://badge.fury.io/nu/ServiceProviderEndpoint.svg)](http://badge.fury.io/nu/ServiceProviderEndpoint)
WebApi endpoint for IServiceProvider


## Features
* Generics support
* Method parameters casting support
* Incoming/outgoing streams


## Example: Send request to MediatR
*.NET CLI*
```
dotnet new web --name "SpeExample"
cd SpeExample
dotnet add package ServiceProviderEndpoint
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

*Change Program.cs*
```C#
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapServiceProvider("services", builder.Services); 

app.Run();
```

*Send request*
```
GET /services/IMediator/Send/Ping?args=[{"Message":"TEST"}]

or

POST /services/IMediator/Send/Ping
Content-Type: application/json
[{"Message":"TEST"}]
```

*Result/Response*
```
{"Message":"TEST PONG"}
```

[See example project for details...](https://github.com/mustaddon/ServiceProviderEndpoint/tree/main/Examples/Example.WebApi)

