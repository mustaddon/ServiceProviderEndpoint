# ServiceProviderEndpoint [![NuGet version](https://badge.fury.io/nu/ServiceProviderEndpoint.svg)](http://badge.fury.io/nu/ServiceProviderEndpoint)
WebApi endpoint for IServiceProvider to improve usability and development speed.


## Usage
If you have already registered services in the collection and want to give access to them via http, then just map a universal endpoint like this:
```C#
app.MapServiceProvider("services", builder.Services);
app.Run();
```

Now you can send post/get requests to your services, like:
```
GET /services/IYourService/SomeMethod?args=["arg1","arg2","arg3"]

or

POST /services/IYourService/SomeMethod
Content-Type: application/json
["arg1","arg2","arg3"]
```


## Generics
Example request with generics:
```
GET /services/IYourService/SomeGenericMethod(Int32)?args=[111,222,333]
```
Requests use URL-safe notation for types **Dictionary(String-Array(Int32))** is equivalent of **Dictionary<string,int[]>** 


## Type casting
If your method has object type arguments like:
```C#
public Task<int> ExampleMethod(object data, CancellationToken cancellationToken) 
```
Then you need to add the type for cast to the request:
```
GET /services/IYourService/ExampleMethod/List(String)?args=[["list_item1","list_item2","list_item3"]]
```


## Security
If you don't want to publish all services in the collection, then just add a filter:
```C#
app.MapServiceProvider("services", builder.Services
	.Where(x => x.ServiceType.Namespace.StartsWith("Example")));
```

If authorization is needed, then it is added by the standard method for IEndpointConventionBuilder:
```C#
app.MapServiceProvider("services", builder.Services)
	.RequireAuthorization();
```

Security for methods can be added via [Scrutor-decorators](https://github.com/khellang/Scrutor):
```C#
builder.Services.AddSingleton<IExampleService, ExampleService>();
builder.Services.Decorate<IExampleService, ExampleServiceSecure>();
```


## .NET client
To connect to api from another .net application, use an existing client:
```C#
using ServiceProviderEndpoint.Client;

using var client = new SpeClient("https://localhost:7149/services");

var result = await client
  .GetService<IYourService>()
  .SomeMethod("arg1","arg2","arg3");
```


[See example project for details...](https://github.com/mustaddon/ServiceProviderEndpoint/tree/main/Examples/Example.WebApi)

