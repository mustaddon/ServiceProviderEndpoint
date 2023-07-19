using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceProviderEndpoint;

public sealed class SpeOptions
{
    public JsonSerializerOptions JsonSerialization { get; } = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
}
