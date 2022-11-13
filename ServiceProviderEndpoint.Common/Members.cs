using System;
using System.Reflection;

namespace ServiceProviderEndpoint;

internal static class Members
{

    public static bool IsExtension(this MethodInfo method) => method.IsDefined(Types.ExtensionAttribute, false);

    public static MethodInfo? MakeGenericMethodSafe(this MethodInfo source, params Type[] typeArguments)
    {
        try
        {
            return source.MakeGenericMethod(typeArguments);
        }
        catch
        {
            return null;
        }
    }

}
