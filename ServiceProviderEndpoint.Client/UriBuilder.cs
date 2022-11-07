using System;
using System.Collections.Generic;
using System.Reflection;
using TypeSerialization;

namespace ServiceProviderEndpoint.Client
{
    internal class UriBuilder
    {
        public static string Build(Type service, MemberInfo member, object?[] args, string? queryArgs = null)
        {
            var paths = new List<string> {
                service.Serialize()
            };

            if (member is MethodInfo method)
                AddMethod(paths, method, args);
            else
                AddProperty(paths, member, args);

            if (queryArgs != null)
                paths.Add($"?args={Uri.EscapeDataString(queryArgs)}");

            return string.Join("/", paths);
        }

        private static void AddProperty(List<string> paths, MemberInfo member, object?[] args)
        {
            paths.Add(member.Name);

            if (args.Length == 0)
                return;

            var argumentType = args[0]?.GetType();

            if (member is PropertyInfo property)
                paths.Add(ChooseParameterType(property.PropertyType, argumentType).Serialize());
            else if (member is FieldInfo field)
                paths.Add(ChooseParameterType(field.FieldType, argumentType).Serialize());
        }

        private static void AddMethod(List<string> paths, MethodInfo method, object?[] args)
        {
            paths.Add(!method.IsGenericMethod ? method.Name
                    : $"{method.Name}({method.GetGenericArguments().Serialize()})");

            var parameters = method.GetParameters();
            var parameterTypes = new Type[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var argumentType = args.Length > i ? args[i]?.GetType() : null;
                parameterTypes[i] = ChooseParameterType(parameterType, argumentType);
            }

            paths.Add(parameterTypes.Serialize());
        }

        private static Type ChooseParameterType(Type parameterType, Type? argumentType)
        {
            if (argumentType == null || argumentType.IsAbstract)
                return parameterType;

            if (!parameterType.IsAbstract && !parameterType.Equals(Types.Object))
                return parameterType;

            if (parameterType.IsStreamable())
                return parameterType;

            return argumentType;
        }
    }
}
