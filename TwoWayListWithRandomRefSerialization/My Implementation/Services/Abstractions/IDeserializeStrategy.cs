namespace Logic_Layer.Services.Abstractions;

public interface IDeserializeStrategy<T>
{
    public ValueTask<T> Deserialize(Stream s);
}