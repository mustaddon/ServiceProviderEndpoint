using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeSerialization;

namespace ServiceProviderEndpoint.Client
{
    internal class UriBuilder
    {
        public static string Build(Type service, MemberInfo member, Type?[]? parameters, object?[] args, string? queryArgs = null)
        {
            var paths = new List<string> {
                service.Serialize()
            };

            if (member is MethodInfo method)
                AddMethod(paths, method, parameters, args);
            else
                AddProperty(paths, member, parameters, args);

            if (queryArgs != null)
                paths.Add($"?args={Uri.EscapeDataString(queryArgs)}");

            return string.Join("/", paths);
        }

        private static void AddProperty(List<string> paths, MemberInfo member, Type?[]? parameters, object?[] args)
        {
            paths.Add(member.Name);

            if (args.Length == 0)
                return;

            if (parameters != null && parameters.Length > 0 && parameters[0] != null)
            {
                paths.Add(parameters[0]!.Serialize());
                return;
            }

            var argumentType = args[0]?.GetType();

            if (member is PropertyInfo property)
                paths.Add(ChooseParameterType(property.PropertyType, argumentType).Serialize());
            else if (member is FieldInfo field)
                paths.Add(ChooseParameterType(field.FieldType, argumentType).Serialize());
        }

        private static void AddMethod(List<string> paths, MethodInfo method, Type?[]? parameters, object?[] args)
        {
            paths.Add(!method.IsGenericMethod ? method.Name
                    : $"{method.Name}({method.GetGenericArguments().Serialize()})");

            var parameterInfos = method.GetParameters();
            var parameterTypes = new Type[parameterInfos.Length];
            var parametersLength = parameters?.Length ?? 0;

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                if (parametersLength > i && parameters![i] != null)
                {
                    parameterTypes[i] = parameters[i]!;
                    continue;
                }

                var parameterType = parameterInfos[i].ParameterType;
                var argumentType = args.Length > i ? args[i]?.GetType() : null;
                parameterTypes[i] = ChooseParameterType(parameterType, argumentType);
            }

            paths.Add(parameterTypes.Skip(method.IsExtension() ? 1 : 0).Serialize());
        }

        private static Type ChooseParameterType(Type parameterType, Type? argumentType)
        {
            if (argumentType == null || argumentType.IsAbstract)
                return parameterType;

            if (!parameterType.IsAbstract && !parameterType.Equals(Types.Object))
                return parameterType;

            if (parameterType.IsStreamable())
                return parameterType;

            if (Types.Type.IsAssignableFrom(argumentType))
                return Types.Type;

            return argumentType;
        }
    }
}
