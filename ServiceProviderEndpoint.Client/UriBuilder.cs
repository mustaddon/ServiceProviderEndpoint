using System;
using System.Collections.Generic;
using System.Reflection;
using TypeSerialization;

namespace ServiceProviderEndpoint.Client
{
    internal class UriBuilder
    {
        public static string Build(Type service, MethodInfo method, object?[] args, string? queryArgs = null)
        {
            var paths = new List<string> {
            service.Serialize()
        };

            paths.Add(!method.IsGenericMethod ? method.Name
                    : $"{method.Name}({method.GetGenericArguments().Serialize()})");

            var parameters = method.GetParameters();
            var parameterTypes = new Type[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                parameterTypes[i] = parameterType;

                if (args.Length > i && args[i] != null
                    && (parameterType.IsAbstract || parameterType.Equals(Types.Object))
                    && !parameterType.IsStreamable())
                {
                    var argType = args[i]!.GetType();

                    if (!argType.IsAbstract)
                        parameterTypes[i] = argType;
                }
            }

            paths.Add(parameterTypes.Serialize());

            if (queryArgs != null)
                paths.Add($"?args={Uri.EscapeDataString(queryArgs)}");

            return string.Join("/", paths);
        }
    }
}
