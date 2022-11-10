using MetaFile;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using TypeSerialization;

namespace ServiceProviderEndpoint;

internal static class TypeDeserializers
{
    public static TypeDeserializer Create(IEnumerable<ServiceDescriptor> services, IEnumerable<Type> types)
    {
        return new TypeDeserializer(services
            .SelectMany(x => new[] { x.ServiceType }
                .Concat(x.ServiceType.GetFields(TypeScanner.Flags)
                    .Select(x => x.FieldType))
                .Concat(x.ServiceType.GetProperties(TypeScanner.Flags)
                    .Select(x => x.PropertyType))
                .Concat(x.ServiceType.GetMethods(TypeScanner.Flags)
                    .SelectMany(x => x.GetParameters())
                    .Select(x => x.ParameterType))
                .Where(x => !x.IsGenericParameter))
            .Concat(types.Where(x => !x.IsStatic()))
            .Concat(SystemTypes)
            .Concat(MetaFileTypes));
    }

    static readonly IEnumerable<Type> SystemTypes = Array.Empty<Type>()
        .Concat(typeof(int).Assembly.GetTypes().Where(x => typeof(IComparable).IsAssignableFrom(x)))
        .Concat(typeof(List<>).Assembly.GetTypes().Where(x => typeof(IEnumerable).IsAssignableFrom(x)))
        .Where(x => x.IsPublic && !x.IsAbstract && !x.IsEnum)
        .Where(x => !Types.Attribute.IsAssignableFrom(x))
        .Where(x => !Types.Exception.IsAssignableFrom(x))
        .Concat(new[] { typeof(object), typeof(int?), typeof(Type), typeof(Stream), typeof(CancellationToken) });

    static readonly IEnumerable<Type> MetaFileTypes = typeof(StreamFile).Assembly.GetTypes()
        .Where(x => x.IsPublic && !x.IsStatic())
        .Where(x => !Types.Attribute.IsAssignableFrom(x))
        .Where(x => !Types.Exception.IsAssignableFrom(x));

}
