using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SingleApi;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeSerialization;

namespace ServiceProviderEndpoint;

class EndpointProcessor
{
    public EndpointProcessor(IEnumerable<ServiceDescriptor> services, IEnumerable<Type> types, SpeOptions options)
    {
        _options = options;
        _typeDeserializer = new TypeDeserializer(services
            .Select(x => x.ServiceType)
            .Concat(types)
            .Concat(typeof(SapiFile).Assembly.GetTypes().Where(x => x.IsPublic && !x.IsStaticOrAttribute()))
            .Concat(new[] { typeof(Stream) }));
    }

    readonly SpeOptions _options;
    readonly TypeDeserializer _typeDeserializer;
    readonly ConcurrentDictionary<string, TypeMember> _typeMembers = new();

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

    Task<IResult> Process(HttpContext ctx, string serviceType, string memberName, string? argumentTypes, JsonArray? args)
    {
        var serviceTypeObj = _typeDeserializer.Deserialize(serviceType);
        var service = ctx.RequestServices.GetRequiredService(serviceTypeObj);
        var memberKey = string.Join("|", serviceType, memberName, argumentTypes, args?.Count);

        if (!_typeMembers.TryGetValue(memberKey, out var typeMember))
        {
            var argumentTypesObj = argumentTypes == null ? null : _typeDeserializer.DeserializeMany(argumentTypes);
            var memberGenericTypes = ExtractGenericTypes(ref memberName);
            typeMember = serviceTypeObj.FindMember(memberName, memberGenericTypes, argumentTypesObj, args?.Count ?? 0)
                ?? throw new Exception($"Member '{memberName}'{memberGenericTypes?.Length} not found.");

            _typeMembers.TryAdd(memberKey, typeMember);
        }

        return GetResult(ctx, typeMember.GetValue(service, ctx, args, _options.JsonSerialization));
    }

    Type[]? ExtractGenericTypes(ref string memberName)
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

        if (value is IResult result)
            return result;

        if (value is ISapiFileReadOnly file)
            return file.ToResult(ctx.Response, _options.JsonSerialization);

        if (value is Stream stream)
            return Results.Stream(stream);

        return Results.Json(value, _options.JsonSerialization);
    }
}
