using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeSerialization;

namespace ServiceProviderEndpoint.Client;

public class SpeClientSettings
{
    internal static readonly SpeClientSettings Default = new();


    public TypeDeserializer TypeDeserializer { get; set; } = TypeDeserializers.Default;

    public ICredentials? Credentials { get; set; }

    public Dictionary<string, IEnumerable<string>> DefaultRequestHeaders { get; set; } = new();

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
}