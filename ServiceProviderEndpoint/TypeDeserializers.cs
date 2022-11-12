using Microsoft.Extensions.DependencyInjection;
using TypeSerialization;

namespace ServiceProviderEndpoint;

internal static class TypeDeserializers
{
    public static TypeDeserializer Create(IEnumerable<ServiceDescriptor> services, IEnumerable<Type> types)
    {
        return new(services
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
            .Concat(Types.Cores)
            .Concat(Types.Systems)
            .Concat(Types.MetaFiles));
    }

}
