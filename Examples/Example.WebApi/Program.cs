using Example;
using Example.WebApi.Services;
using MediatR;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// MediatR registration
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Other services registration
builder.Services.AddScoped<ExampleService>();
builder.Services.AddScoped<IExampleService, ExampleService>();
builder.Services.AddScoped(typeof(IExampleGenericService<>), typeof(ExampleGenericService<>));


var app = builder.Build();

// Endpoint mapping
app.MapServiceProvider("services", builder.Services
    // add a filter if needed
    .Where(x => x.ServiceType != typeof(IConfiguration)),
    // additional assemblies for types casting and generics resolving
    typeof(int).Assembly, typeof(List<>).Assembly);

app.Run();