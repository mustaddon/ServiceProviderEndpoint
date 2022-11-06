using SingleApi;
using System.Reflection;

namespace ServiceProviderEndpoint;

internal static class TypeScanner
{
    public const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;

    public static TypeMember? FindMember(this Type type, string name, Type[]? genericTypes, Type[]? argumentTypes, int argsCount, bool recursionless = false)
    {
        var isGeneric = genericTypes != null;

        if (!isGeneric && argsCount == 0)
        {
            if (type.GetProperty(name, Flags) is PropertyInfo propertyInfo)
                return new(propertyInfo);
            else if (type.GetField(name, Flags) is FieldInfo fieldInfo)
                return new(fieldInfo);
        }

        var methods = type.GetMethods(Flags)
            .Where(x => x.Name == name && x.IsGenericMethod == isGeneric);

        if (isGeneric)
            methods = methods
                .Where(x => x.GetGenericArguments().Length == genericTypes!.Length)
                .Select(x => x.MakeGenericMethod(genericTypes!));

        var extendedMethods = methods
            .Select(x =>
            {
                var parameters = x.GetParameters();

                return new
                {
                    Method = x,
                    Parameters = parameters,
                    RequiredParamsCount = parameters.Where(x => !x.IsOptional && !x.ParameterType.IsAutoFillable()).Count(),
                };
            })
            .Where(x => argsCount <= x.Parameters.Length && argsCount >= x.RequiredParamsCount);

        if (argumentTypes != null)
            extendedMethods = extendedMethods.Where(x => CheckParameterTypes(x.Parameters, argumentTypes));

        foreach (var x in extendedMethods.OrderByDescending(x => x.Parameters.Length))
            return new(x.Method, x.Parameters.ApplyArgumentTypes(argumentTypes));

        if (type.IsInterface && !recursionless)
            return type.GetInterfaces()
                .Select(x => x.FindMember(name, genericTypes, argumentTypes, argsCount, true))
                .FirstOrDefault(x => x != null);

        return null;
    }

    static bool CheckParameterTypes(ParameterInfo[] parameters, Type[] arguments)
    {
        if (parameters.Length < arguments.Length)
            return false;

        for (var i = 0; i < arguments.Length; i++)
            if (!parameters[i].ParameterType.IsAssignableFrom(arguments[i]))
                return false;

        return true;
    }

    static TypeMemberParameter[] ApplyArgumentTypes(this ParameterInfo[] parameters, Type[]? arguments)
    {
        var result = new TypeMemberParameter[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
            result[i] = new(arguments?.Length > i ? arguments[i] : parameters[i].ParameterType,
                DefaultValue: parameters[i].DefaultValue);

        return result;
    }

    
}
