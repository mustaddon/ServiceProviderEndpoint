using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TypeSerialization;

namespace ServiceProviderEndpoint;

internal class JsonTypeConverter : JsonConverterFactory
{
    public JsonTypeConverter(TypeDeserializer typeDeserializer)
    {
        _typeConverter = new InnerConverter(typeDeserializer);
    }

    readonly InnerConverter _typeConverter;

    public override bool CanConvert(Type typeToConvert)
    {
        return Types.Type.IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _typeConverter;
    }

    class InnerConverter : JsonConverter<Type>
    {
        public InnerConverter(TypeDeserializer typeDeserializer)
        {
            _typeDeserializer = typeDeserializer;
        }

        readonly TypeDeserializer _typeDeserializer;

        public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var valStr = reader.GetString();
            return valStr == null ? null : _typeDeserializer.Deserialize(valStr);
        }

        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.Serialize());
        }
    }
}
