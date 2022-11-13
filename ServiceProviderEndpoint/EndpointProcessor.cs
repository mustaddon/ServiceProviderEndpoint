using MetaFile;
using MetaFile.Http.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeSerialization;
using TypeSerialization.Json;

namespace ServiceProviderEndpoint;

class EndpointProcessor
{
    public EndpointProcessor(IEnumerable<ServiceDescriptor> services, IEnumerable<Type> extensions, SpeOptions options)
    {
        _options = options;
        _memberProvider = new(services, extensions);
        _typeDeserializer = TypeDeserializers.Create(services, extensions);
        _options.JsonSerialization.Converters.Add(new JsonTypeConverter(_typeDeserializer));
        _serviceTypes = new(services.Select(x => x.ServiceType));
    }

    readonly SpeOptions _options;
    readonly MemberProvider _memberProvider;
    readonly TypeDeserializer _typeDeserializer;
    readonly ConcurrentDictionary<string, TypeMember> _typeMembers = new();
    readonly HashSet<Type> _serviceTypes;

    public Task<IResult> ProcessGet(HttpContext ctx, string service, string member, string? parameters, string? args)
    {
        var argsObj = args == null ? null : JsonSerializer.Deserialize<JsonArray>(args, _options.JsonSerialization);

        ctx.Response.Headers.AddNoCache();

        return Process(ctx, service, member, parameters, argsObj);
    }

    public async Task<IResult> ProcessPost(HttpContext ctx, string service, string member, string? parameters, string? args)
    {
        var argsObj = args != null ? JsonSerializer.Deserialize<JsonArray>(args, _options.JsonSerialization)
            : ctx.Request.IsJson() ? await JsonSerializer.DeserializeAsync<JsonArray>(ctx.Request.Body, _options.JsonSerialization, ctx.RequestAborted)
            : null;

        return await Process(ctx, service, member, parameters, argsObj);
    }

    Task<IResult> Process(HttpContext ctx, string serviceName, string memberName, string? paramererNames, JsonArray? args)
    {
        var serviceTypeObj = _typeDeserializer.Deserialize(serviceName)!;
        var service = ctx.RequestServices.GetService(serviceTypeObj) ?? throw new InvalidOperationException($"Service '{serviceName}' not found.");
        var memberKey = string.Join("|", serviceName, memberName, paramererNames, args?.Count);

        if (!_typeMembers.TryGetValue(memberKey, out var typeMember))
        {
            var parameters = paramererNames == null ? null : _typeDeserializer.DeserializeMany(paramererNames);
            var memberGenericArgs = ExtractGenericTypes(ref memberName);
            typeMember = _memberProvider.GetMember(serviceTypeObj, memberName, memberGenericArgs, parameters, args?.Count ?? 0)
                ?? throw new InvalidOperationException($"Member '{memberName}'{memberGenericArgs?.Length} not found.");

            _typeMembers.TryAdd(memberKey, typeMember);
        }

        return GetResult(ctx, typeMember.GetValue(service, ctx, args, _options.JsonSerialization, _typeDeserializer));
    }

    Type?[]? ExtractGenericTypes(ref string memberName)
    {
        var genericIndex = memberName.IndexOf('(');

        if (genericIndex < 0)
            return null;

        var genericArgs = memberName[(genericIndex + 1)..(memberName.Length - 1)];

        memberName = memberName[..genericIndex];

        return _typeDeserializer.DeserializeMany(genericArgs);
    }

    async Task<IResult> GetResult(HttpContext ctx, Task<object?> valueTask)
    {
        var value = await valueTask;

        if (value == null)
            return Results.NoContent();

        ctx.Response.Headers[Headers.ResultType] = value.GetType().Serialize();

        if (value is IResult result)
            return result;

        if (value is IStreamFileReadOnly file)
            return file.ToResult(ctx.Response, _options.JsonSerialization);

        if (value is Stream stream)
            return Results.Stream(stream);

        return Results.Json(value, _options.JsonSerialization);
    }
}
