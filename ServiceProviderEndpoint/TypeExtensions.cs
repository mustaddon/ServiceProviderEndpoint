namespace ServiceProviderEndpoint;

internal static class TypeExtensions
{
    public static bool IsStaticOrAttribute(this Type type)
    {
        return (type.IsAbstract && type.IsSealed) || type.IsAssignableTo(typeof(Attribute));
    }
}
