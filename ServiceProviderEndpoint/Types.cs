using MetaFile;

namespace ServiceProviderEndpoint;

internal static class Types
{
    public static readonly Type Type = typeof(Type);
    public static readonly Type Void = typeof(void);
    public static readonly Type Object = typeof(object);
    public static readonly Type Attribute = typeof(Attribute);
    public static readonly Type Exception = typeof(Exception);
    public static readonly Type Task = typeof(Task);
    public static readonly Type Stream = typeof(Stream);
    public static readonly Type StreamFile = typeof(StreamFile);
    public static readonly Type IStreamFile = typeof(IStreamFile);
    public static readonly Type IStreamFileReadOnly = typeof(IStreamFileReadOnly);
    public static readonly Type CancellationToken = typeof(CancellationToken);


    public static bool IsStatic(this Type type) => type.IsAbstract && type.IsSealed;

    public static bool IsStreamable(this Type type) => Stream.IsAssignableFrom(type) || IStreamFileReadOnly.IsAssignableFrom(type);

    public static bool IsAutoFillable(this Type type)
    {
        return type.Equals(Stream)
            || type.IsAssignableFrom(CancellationToken)
            || type.IsAssignableFrom(StreamFile)
            || (!type.IsAbstract && IStreamFile.IsAssignableFrom(type));
    }
}
