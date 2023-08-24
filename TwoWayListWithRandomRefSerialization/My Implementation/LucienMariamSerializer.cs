using Logic_Layer.Services;
using Logic_Layer.Services.Abstractions;
using Logic_Layer.Strategies;
using Task_Original_Files;

namespace Logic_Layer;

//Specify your class\file name and complete implementation.
public class LucienMariamSerializer : IListSerializer
{
    private const string STREAM_IS_INVALID = "Stream is invalid: null, empty, wrong format or inner structure!";
    private const string LIST_IS_INVALID = "Collection of ListNodes is null";

    private readonly RetryProvider retryProvider;
    private readonly IDeepCopyStrategy<ListNode> deepCopyArrayBasedAlgorithm;
    private readonly ISerializeStrategy<ListNode> serializeAlgorithm;

    public IDeserializeStrategy<ListNode> DeserializeAlgorithm { get; set; }

    //the constructor with no parameters is required and no other constructors can be used.
    public LucienMariamSerializer()
    {
        serializeAlgorithm = new SerializeListNode4CyclesWithParallelStrategy();
        deepCopyArrayBasedAlgorithm = new DeepCopyListNodeArrayStrategy();
        DeserializeAlgorithm = new DeserializeListNodeStrategy();
        retryProvider = new RetryProvider();
    }
    
    /// <summary>
    /// Serializes all nodes in the list, including topology of the Random links, into stream.
    /// </summary>
    public async Task Serialize(ListNode head, Stream s)
    {
        try
        {
            if (head == null)
                throw new ArgumentException(LIST_IS_INVALID);

            if (s == null)
                throw new ArgumentException(STREAM_IS_INVALID);

            if (!s.CanWrite)
                await retryProvider.RetryStreamWrite(s);

            await serializeAlgorithm.Serialize(head, s);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            if (s != null)
                await s.DisposeAsync();
            
            throw;
        }
    }

    /// <summary>
    /// Deserializes the list from the stream, returns the head node of the list.
    /// </summary>
    /// <exception cref="System.ArgumentException">Thrown when a stream has invalid data.</exception>
    public async Task<ListNode> Deserialize(Stream s)
    {
        try
        {
            if (s == null || s.Length == 0)
                throw new ArgumentException(STREAM_IS_INVALID);

            if (!s.CanRead)
                await retryProvider.RetryStreamRead(s);

            return await DeserializeAlgorithm.Deserialize(s);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            if (s != null)
                await s.DisposeAsync();

            throw;
        }
    }
    
    /// <summary>
    /// Makes a deep copy of the list, returns the head node of the list.
    /// </summary>
    public Task<ListNode> DeepCopy(ListNode head)
    {
        ListNode newHead = null;
        
        try
        {
            if (head == null)
                throw new ArgumentException(LIST_IS_INVALID);

            newHead = deepCopyArrayBasedAlgorithm.DeepCopy(head);
            return Task.FromResult(newHead);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            if (newHead == null || newHead.Next == null)
                throw;

            newHead = null;
            GC.Collect(0, GCCollectionMode.Optimized, false, false);
            throw;
        }
    }
}