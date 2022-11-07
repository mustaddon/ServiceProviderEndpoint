using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ServiceProviderEndpoint.Client;

internal class ArgsBuilder
{
    public static List<object?> Build(MemberInfo member, object?[] args, out List<object> streamables, out List<CancellationToken> cTockens)
    {
        var result = new List<object?>();
        streamables = new List<object>();
        cTockens = new List<CancellationToken>();

        if (member is MethodInfo method)
        {
            var parameters = method.GetParameters();
            var count = Math.Min(args.Length, parameters.Length);

            for (var i = 0; i < count; i++)
                Add(parameters[i].ParameterType, args[i], result, streamables, cTockens);
        }
        else if (args.Length > 0)
        {
            if (member is PropertyInfo property)
                Add(property.PropertyType, args[0], result, streamables, cTockens);
            else if (member is FieldInfo field)
                Add(field.FieldType, args[0], result, streamables, cTockens);
        }

        return result;
    }


    private static void Add(Type parameterType, object? arg, List<object?> result, List<object> streamables, List<CancellationToken> cTockens)
    {
        if (!parameterType.Equals(Types.Object))
        {
            if (parameterType.IsAssignableFrom(Types.CancellationToken))
            {
                if (arg is CancellationToken cancellationToken)
                    cTockens.Add(cancellationToken);

                return;
            }

            if (parameterType.IsStreamable())
            {
                streamables.Add(arg ?? Stream.Null);
                return;
            }
        }

        result.Add(arg);
    }
}
