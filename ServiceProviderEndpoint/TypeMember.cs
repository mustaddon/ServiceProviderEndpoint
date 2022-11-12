using MetaFile.Http.AspNetCore;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using TypeSerialization;

namespace ServiceProviderEndpoint;


internal record TypeMember(MemberInfo Info, TypeMemberParameter[]? Parameters);
internal record TypeMemberParameter(Type Type, object? DefaultValue = null);


internal static class TypeMemberExtensions
{
    public static async Task<object?> GetValue(this TypeMember member, object? obj, HttpContext ctx, JsonArray? args, JsonSerializerOptions jsonOptions, TypeDeserializer typeDeserializer)
    {
        object? result = null;

        if (member.Info is MethodInfo methodInfo)
        {
            result = methodInfo.Invoke(obj, member.Parameters!.GetArguments(ctx, args, jsonOptions, typeDeserializer));
        }
        else if (member.Info is PropertyInfo propertyInfo)
        {
            if (args?.Any() == true)
                propertyInfo.SetValue(obj, member.Parameters!.GetArguments(ctx, args, jsonOptions, typeDeserializer)[0]);
            else
                result = propertyInfo.GetValue(obj);
        }
        else if (member.Info is FieldInfo fieldInfo)
        {
            if (args?.Any() == true)
                fieldInfo.SetValue(obj, member.Parameters!.GetArguments(ctx, args, jsonOptions, typeDeserializer)[0]);
            else
                result = fieldInfo.GetValue(obj);
        }

        if (result is ValueTask valueTask)
            result = valueTask.AsTask();

        if (result is not Task task)
            return result;

        await task.ConfigureAwait(false);

        return task.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(task);
    }

    static object?[] GetArguments(this TypeMemberParameter[] parameters, HttpContext ctx, JsonArray? args, JsonSerializerOptions jsonOptions, TypeDeserializer typeDeserializer)
    {
        var result = new object?[parameters.Length];
        var argsIndex = 0;
        var argsCount = args?.Count ?? 0;

        for (var i = 0; i < result.Length; i++)
        {
            var type = parameters[i].Type;

            if (type.Equals(Types.Stream))
            {
                result[i] = new SpeStream(ctx.Request);
                continue;
            }

            if (!type.Equals(Types.Object))
            {
                if (type.IsAssignableFrom(Types.CancellationToken))
                {
                    result[i] = ctx.RequestAborted;
                    continue;
                }

                if (type.IsAssignableFrom(Types.StreamFile))
                {
                    result[i] = ctx.Request.ToStreamFile(Types.StreamFile, jsonOptions);
                    continue;
                }

                if (!type.IsAbstract && Types.IStreamFile.IsAssignableFrom(type))
                {
                    result[i] = ctx.Request.ToStreamFile(type, jsonOptions);
                    continue;
                }
            }

            if (argsIndex < argsCount)
            {
                result[i] = args![argsIndex++].Deserialize(type, jsonOptions);
                continue;
            }

            result[i] = parameters[i].DefaultValue;
        }

        return result;
    }
}