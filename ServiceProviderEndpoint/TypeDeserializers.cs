using Microsoft.Extensions.DependencyInjection;
using TypeSerialization;

namespace ServiceProviderEndpoint;

internal static class TypeDeserializers
{
    public static TypeDeserializer Create(IEnumerable<Type> services, IEnumerable<Type> types)
    {
        return new(services
            .SelectMany(x => new[] { x }
                .Concat(x.GetFields(MemberProvider.Flags)
                    .Select(xx => xx.FieldType))
                .Concat(x.GetProperties(MemberProvider.Flags)
                    .Select(xx => xx.PropertyType))
                .Concat(x.GetMethods(MemberProvider.Flags)
                    .SelectMany(xx => xx.GetParameters())
                    .Select(xx => xx.ParameterType))
                .Where(xx => !xx.IsGenericParameter))
            .Concat(types.Where(x => !x.IsStatic()))
            .Concat(Types.Cores)
            .Concat(Types.Systems.Value)
            .Concat(Types.MetaFiles.Value));
    }

}
