using MediatR;
using System.Reflection;
using Test.Requests;
using Test.Services;
using Test.WebApi.Handlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SimpleService>();
builder.Services.AddSingleton<ISimpleService>(x => x.GetRequiredService<SimpleService>());

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<IRequestHandler<FileUpload<FileMetadata>, FileUploadResult<FileMetadata>>, FileUploadHandler<FileMetadata>>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

var app = builder.Build();

app.MapServiceProvider("services", builder.Services);

app.Run();