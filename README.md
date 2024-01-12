# ServiceProviderEndpoint [![NuGet version](https://badge.fury.io/nu/ServiceProviderEndpoint.svg?)](http://badge.fury.io/nu/ServiceProviderEndpoint)
IServiceProvider webapi endpoint for faster and easier development.


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
Requests use URL-safe notation for types. For example, **Dictionary(String-Array(Int32))** is equivalent of **Dictionary<string,int[]>**. 


## Type casting
If your method has object type arguments like:
```C#
Task<int> ExampleMethod(object data, CancellationToken cancellationToken);
```
Then you need to add the type for cast as an additional parameter to the request:
```
GET /services/IYourService/ExampleMethod/List(String)?args=[["list_item1","list_item2","list_item3"]]
```


## File streams
For downloading, it is enough that the method returns a stream object:
```C#
Task<Stream> SomeDownloadMethod(string a, string b, string c, CancellationToken cancellationToken);
```
Download request will be like this:
```
GET /services/IYourService/SomeDownloadMethod?args=["argA","argB","argC"]
```


For uploading, the method must have an argument of type Stream (position doesn't matter):
```C#
Task SomeUploadMethod(Stream stream, string a, string b, string c, CancellationToken cancellationToken); 
```
Upload request:
```
POST /services/IYourService/SomeUploadMethod?args=["argA","argB","argC"]
Content-Type: application/octet-stream
<SomeFileData>
```
JavaScript example:
```js
let file = document.getElementById('some-input').files[0];
let response = await fetch('/services/IYourService/SomeUploadMethod?args='+encodeURIComponent(JSON.stringify(["argA","argB","argC"])), {
  method: 'POST',
  headers: { 'content-type': file.type || 'application/octet-stream' },
  body: file,
});
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
builder.Services.AddScoped<IExampleService, ExampleService>();
builder.Services.Decorate<IExampleService, SecureExampleService>();
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


## Example projects
* [WebApi](https://github.com/mustaddon/ServiceProviderEndpoint/tree/main/Examples/Example.WebApi/Program.cs)
* [ClientApp](https://github.com/mustaddon/ServiceProviderEndpoint/tree/main/Examples/Example.Client/Program.cs)

