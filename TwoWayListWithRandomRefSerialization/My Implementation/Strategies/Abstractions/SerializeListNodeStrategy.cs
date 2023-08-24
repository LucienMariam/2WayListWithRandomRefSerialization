using Logic_Layer.Models;
using Logic_Layer.Services.Abstractions;
using Task_Original_Files;
using Utf8Json;

namespace Logic_Layer.Strategies.Abstractions;

public abstract class SerializeListNodeStrategy : ISerializeStrategy<ListNode>
{
    private const byte ARRAY_NODE_SEPARATOR = 44;
    private const byte ARRAY_START_BYTE = 91;
    private const byte ARRAY_END_BYTE = 93;
    
    public virtual async Task Serialize(ListNode head, Stream s)
    {
        await JsonSerializer.SerializeAsync(s, ARRAY_START_BYTE);

        if (head.Next != null)
        {
            await this.HandleList(head, s);
        }
        else await HandleOneElementList(head, s);

        await JsonSerializer.SerializeAsync(s, ARRAY_END_BYTE);
    }

    public abstract Task HandleList(ListNode head, Stream s);
    
    protected async Task Serialize(ListNodeJsonModel[] jsonModels, Stream s)
    {
        int lengthWithoutLastElement = jsonModels.Length - 1;

        for (int i = 0; i < lengthWithoutLastElement; i++)
        {
            await JsonSerializer.SerializeAsync(s, jsonModels[i]);
            await JsonSerializer.SerializeAsync(s, ARRAY_NODE_SEPARATOR);
        }
        
        await JsonSerializer.SerializeAsync(s, jsonModels[lengthWithoutLastElement]);
    }
    
    private static Task HandleOneElementList(ListNode node, Stream s)
    {
        var jsonNode = new ListNodeJsonModel
        {
            Id = 1,
            Data = node.Data,
            Random = node.Random == null ? null : 1
        };

        return JsonSerializer.SerializeAsync(s, jsonNode);
    }
}