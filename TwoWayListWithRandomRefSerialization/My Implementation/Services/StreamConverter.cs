namespace Logic_Layer.Services;

public static class StreamConverter
{
    public static async Task<byte[]> ToByteArray(Stream s)
    {
        long streamSize = s.Length;
        var byteArray = new byte[streamSize];
        Memory<byte> memory = byteArray.AsMemory(0, (int)streamSize);
        await s.ReadAsync(memory);
        return byteArray;
    }
}