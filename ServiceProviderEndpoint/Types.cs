using SingleApi;

namespace ServiceProviderEndpoint;

internal static class Types
{
    public static readonly Type Void = typeof(void);
    public static readonly Type Object = typeof(object);
    public static readonly Type Attribute = typeof(Attribute);
    public static readonly Type Task = typeof(Task);
    public static readonly Type Stream = typeof(Stream);
    public static readonly Type SapiFile = typeof(SapiFile);
    public static readonly Type ISapiFile = typeof(ISapiFile);
    public static readonly Type ISapiFileReadOnly = typeof(ISapiFileReadOnly);
    public static readonly Type CancellationToken = typeof(CancellationToken);


    public static bool IsStaticOrAttribute(this Type type) => (type.IsAbstract && type.IsSealed) || Attribute.IsAssignableFrom(type);

    public static bool IsStreamable(this Type type) => Stream.IsAssignableFrom(type) || ISapiFileReadOnly.IsAssignableFrom(type);

    public static bool IsAutoFillable(this Type type)
    {
        return type.Equals(Stream) 
            || type.IsAssignableFrom(CancellationToken)
            || type.IsAssignableFrom(SapiFile)
            || (!type.IsAbstract && ISapiFile.IsAssignableFrom(type));
    }
}
