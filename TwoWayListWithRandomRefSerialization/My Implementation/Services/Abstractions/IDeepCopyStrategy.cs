namespace Logic_Layer.Services.Abstractions;

public interface IDeepCopyStrategy<T>
{
    public T DeepCopy(T copyFrom);
}