using Example;
using Example.WebApi.Services;
using MediatR;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

// MediatR registration
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// simple service
builder.Services.AddSingleton<IExampleService, ExampleService>();
builder.Services.Decorate<IExampleService, SecureExampleService>();
// generic open service
builder.Services.AddScoped(typeof(IExampleGenericService<>), typeof(ExampleGenericService<>));


var app = builder.Build();

// Endpoint mapping
app.MapServiceProvider("services", builder.Services
    // add a filter if you need
    .Where(x => x.ServiceType != typeof(IConfiguration)),
    // add types for extensions, casting or resolving 
    new [] { typeof(ExampleServiceExtensions) }
);

app.Run();