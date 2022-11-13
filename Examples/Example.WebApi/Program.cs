using Example;
using Example.WebApi.Services;
using MediatR;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// MediatR registration
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Other test services registration
builder.Services.AddSingleton<IExampleService, ExampleService>();
builder.Services.AddScoped(typeof(IExampleGenericService<>), typeof(ExampleGenericService<>));


var app = builder.Build();

// Endpoint mapping
app.MapServiceProvider("services", builder.Services
    // add a filter if you need
    .Where(x => x.ServiceType != typeof(IConfiguration)));

app.Run();