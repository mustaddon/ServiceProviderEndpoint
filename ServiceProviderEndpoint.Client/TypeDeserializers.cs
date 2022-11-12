using System.Linq;
using TypeSerialization;

namespace ServiceProviderEndpoint.Client;

internal static class TypeDeserializers
{
    public static readonly TypeDeserializer Default = new(Types.Cores
        .Concat(Types.Systems)
        .Concat(Types.MetaFiles));
}
