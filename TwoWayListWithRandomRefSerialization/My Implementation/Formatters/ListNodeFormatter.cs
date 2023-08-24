using Logic_Layer.Models;
using Utf8Json;

namespace Logic_Layer.Formatters;

public sealed class ListNodeFormatter : IJsonFormatter<ListNodeJsonModel>
{
    private const string Id = "id";
    private const string Data = "Data";
    private const string Random = "Random";
    
    public void Serialize(ref JsonWriter writer, ListNodeJsonModel value, IJsonFormatterResolver formatterResolver)
    {
        writer.WriteBeginObject();

        writer.WriteString(Id);
        writer.WriteNameSeparator();
        writer.WriteInt32(value.Id);
        writer.WriteValueSeparator();
        
        writer.WriteString(Data);
        writer.WriteNameSeparator();
        writer.WriteString(value.Data);
        writer.WriteValueSeparator();

        writer.WriteString(Random);
        writer.WriteNameSeparator();
        if (value.Random.HasValue)
            writer.WriteInt32(value.Random.Value);
        else writer.WriteNull();

        writer.WriteEndObject();
    }

    public ListNodeJsonModel Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
        GoToNextReadPayload(ref reader);
        string data = reader.ReadString();
        
        while(!reader.ReadIsNameSeparator())
            reader.ReadNext();
        
        return new ListNodeJsonModel
        {
            Data = data,
            Random = reader.ReadIsNull() ? null : reader.ReadInt32()
        };
    }

    private void GoToNextReadPayload(ref JsonReader reader)
    {
        while (!reader.ReadIsBeginObject())
            reader.ReadNext();
        
        while (!reader.ReadIsValueSeparator())
            reader.ReadNext();
        
        while(!reader.ReadIsNameSeparator())
            reader.ReadNext();
    }
}