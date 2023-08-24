namespace Logic_Layer.Services.Abstractions;

public interface ISerializeStrategy<T>
{
    public Task Serialize(T node, Stream s);
}