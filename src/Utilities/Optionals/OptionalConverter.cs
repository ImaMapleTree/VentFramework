/*using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VentLib.Utilities.Optionals;

public class OptionalConverter : JsonConverterFactory
{
    public override Optional<dynamic> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        dynamic? obj = JsonSerializer.Deserialize<dynamic>(reader: ref reader, options);
        return obj == null ? Optional<dynamic>.Null() : Optional<dynamic>.Of(obj);
    }

    public override void Write(Utf8JsonWriter writer, Optional<dynamic> value, JsonSerializerOptions options)
    {
        if (!value.Exists())
        {
            writer.WriteNullValue();
            return;
        }

        dynamic obj = value.Get();
        ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType(), options));
        writer.WriteRawValue(bytes);
    }

    public override bool CanConvert(Type typeToConvert)
    {
        throw new NotImplementedException();
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}*/