using MediatR;
using System.Reflection;
using Test.Requests;
using Test.Services;
using Test.WebApi.Handlers;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<SimpleService>();
builder.Services.AddSingleton<ISimpleService>(x => x.GetRequiredService<SimpleService>());

builder.Services.AddSingleton<IGenericService<int>, GenericService<int>>();
builder.Services.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient<IRequestHandler<FileUpload<FileMetadata>, FileUploadResult<FileMetadata>>, FileUploadHandler<FileMetadata>>();

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = long.MaxValue);

var app = builder.Build();

app.MapServiceProvider("services",
    builder.Services.Where(x => x.ServiceType.Namespace!.StartsWith("Test")),
    new[] {
        typeof(SimpleServiceExtensions),
        typeof(GenericServiceExtensions),
    });

app.Run();