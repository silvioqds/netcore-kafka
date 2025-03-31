using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using NETKafka.Common.Events;

public class EventJsonConverter : JsonConverter<Event>
{
    public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("EventType", out var typeProperty))
        {
            throw new JsonException("Evento not defined");
        }

        string typeName = typeProperty.GetString();
        Type eventType = Type.GetType(typeName);

        if (eventType == null)
        {
            throw new JsonException($"Type of event unknown: {typeName}");
        }

        return (Event)JsonSerializer.Deserialize(root.GetRawText(), eventType, options);
    }

    public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("EventType", value.GetType().AssemblyQualifiedName);
        foreach (var prop in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object propValue = prop.GetValue(value);
            writer.WritePropertyName(prop.Name);
            JsonSerializer.Serialize(writer, propValue, options);
        }

        writer.WriteEndObject();
    }
}
