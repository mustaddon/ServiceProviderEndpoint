using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeSerialization;

namespace ServiceProviderEndpoint.Client;

public class SpeClientSettings
{

    public ICredentials? Credentials { get; set; }

    public Dictionary<string, IEnumerable<string>> DefaultRequestHeaders { get; set; } = new();

    public TypeDeserializer? TypeDeserializer { get; set; } = DefaultTypeDeserializer;

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };


    internal static readonly SpeClientSettings Default = new();
    private static readonly TypeDeserializer DefaultTypeDeserializer = new(
        new[] { typeof(Stream) });
}
