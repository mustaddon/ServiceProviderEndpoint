using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ServiceProviderEndpoint;

internal class MemberProvider
{
    public MemberProvider(IEnumerable<Type> serviceTypes, IEnumerable<Type> extensions)
    {
        _extensions = TypeExtsMap.Create(serviceTypes, extensions);
    }

    readonly Dictionary<Type, HashSet<MethodInfo>> _extensions;

    public const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;

    public TypeMember? GetMember(Type serviceType, string name, Type?[]? genericArgs, Type?[]? argumentTypes, int argsCount, bool recursionless = false)
    {
        var isGeneric = genericArgs != null;

        if (!isGeneric)
        {
            if (serviceType.GetProperty(name, Flags) is PropertyInfo propertyInfo)
                return new(propertyInfo, ApplyArgumentType(propertyInfo.PropertyType, argumentTypes));
            else if (serviceType.GetField(name, Flags) is FieldInfo fieldInfo)
                return new(fieldInfo, ApplyArgumentType(fieldInfo.FieldType, argumentTypes));
        }

        var methods = serviceType.GetMethods(Flags)
            .Concat(GetExtensionMethods(serviceType))
            .Where(x => x.Name == name && x.IsGenericMethod == isGeneric);

        if (isGeneric)
            methods = methods
                .Where(x => x.GetGenericArguments().Length == genericArgs!.Length)
                .Select(x => x.MakeGenericMethodSafe(genericArgs!)!)
                .Where(x => x != null);

        var extendedMethods = methods
            .Select(x => new MethodWrapper(x, serviceType))
            .Where(x => argsCount <= x.Parameters.Length && argsCount >= x.RequiredParamsCount);

        if (argumentTypes != null)
            extendedMethods = extendedMethods.Where(x => CheckParameterTypes(x, argumentTypes));

        foreach (var x in extendedMethods.OrderByDescending(x => x.ParamsCount))
            return new(x.Method, ApplyArgumentTypes(x, argumentTypes), x.IsExtension);

        if (serviceType.IsInterface && !recursionless)
            return serviceType.GetInterfaces()
                .Select(x => GetMember(x, name, genericArgs, argumentTypes, argsCount, true))
                .FirstOrDefault(x => x != null);

        return null;
    }

    IEnumerable<MethodInfo> GetExtensionMethods(Type type)
    {
        var result = _extensions.TryGetValue(type, out var typeExts) ? typeExts
            : Array.Empty<MethodInfo>().AsEnumerable();

        if (type.IsConstructedGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();

            if (_extensions.TryGetValue(genericType, out var genericOpenExts))
                result = result.Concat(genericOpenExts);
        }

        return result;
    }

    static bool CheckParameterTypes(MethodWrapper method, Type?[] arguments)
    {
        if (method.ParamsCount < arguments.Length)
            return false;

        for (int i = method.IsExtension ? 1 : 0, j = 0; j < arguments.Length; i++, j++)
            if (!method.Parameters[i].ParameterType.IsAssignableFrom(arguments[j]))
                return false;

        return true;
    }

    static TypeMemberParameter[] ApplyArgumentTypes(MethodWrapper method, Type?[]? arguments)
    {
        var result = new TypeMemberParameter[method.Parameters.Length];
        var argumentsLength = arguments?.Length ?? 0;

        for (int i = 0, j = method.IsExtension ? -1 : 0; i < method.Parameters.Length; i++, j++)
        {
            var parameter = method.Parameters[i];
            var argumentType = argumentsLength > j && j >= 0 ? arguments![j] : null;
            result[i] = new(argumentType ?? parameter.ParameterType, parameter.DefaultValue);
        }

        return result;
    }

    static TypeMemberParameter[] ApplyArgumentType(Type type, Type?[]? arguments)
    {
        return new[] { new TypeMemberParameter(arguments?.FirstOrDefault() ?? type) };
    }

    class MethodWrapper
    {
        public MethodWrapper(MethodInfo x, Type serviceType)
        {
            Method = x;
            IsExtension = x.IsExtension();
            Parameters = x.GetParameters();
            ParamsCount = IsExtension ? Parameters.Length - 1 : Parameters.Length;
            RequiredParamsCount = Parameters.Where(x => !x.IsOptional && !x.ParameterType.IsAutoFillable(serviceType)).Count();
        }

        public MethodInfo Method { get; }
        public ParameterInfo[] Parameters { get; }
        public int RequiredParamsCount { get; }
        public int ParamsCount { get; }
        public bool IsExtension { get; }
    }
}
