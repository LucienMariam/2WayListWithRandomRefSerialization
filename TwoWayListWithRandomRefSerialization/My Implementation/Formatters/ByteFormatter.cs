using Utf8Json;

namespace Logic_Layer.Formatters;

public class ByteFormatter : IJsonFormatter<byte>
{
    public void Serialize(ref JsonWriter writer, byte value, IJsonFormatterResolver formatterResolver)
    {
        writer.WriteRaw(value);
    }

    public byte Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
        throw new NotImplementedException();
    }
}