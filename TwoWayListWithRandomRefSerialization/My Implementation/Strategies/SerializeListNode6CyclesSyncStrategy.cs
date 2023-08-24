using Logic_Layer.Models;
using Logic_Layer.Services.Abstractions;
using Logic_Layer.Strategies.Abstractions;
using Task_Original_Files;
namespace Logic_Layer.Strategies;

/// <summary>
/// Algorithm specialized for short lists (< 750 'nodes')
/// </summary>
/// <remarks>
/// Big O = 2n + 2nlogn = 2n(1 + logn)
/// 6 сycles: 1 + 1*1 + 1*1 + 1
/// </remarks>
public class SerializeListNode6CyclesSyncStrategy : SerializeListNodeStrategy, ISerializeStrategy<ListNode>
{
    public const int EFFECTIVE_LIST_SIZE_LIMIT = 750;
    
    private readonly int? listSize;

    public SerializeListNode6CyclesSyncStrategy(int? listSize = null)
    {
        this.listSize = listSize;
    }

    public override Task HandleList(ListNode head, Stream s)
    {
        ListNodeJsonModel[] jsonModels = ConvertToArrayWithIncompleteRandoms(head);
        CompleteRandoms(head, jsonModels);
        return this.Serialize(jsonModels, s);
    }

    private static void CompleteRandoms(ListNode head, ListNodeJsonModel[] jsonModels)
    {
        var resultNodes = new Span<ListNodeJsonModel>(jsonModels);

        for (int i = 0, ip = 1; i < resultNodes.Length; i++, ip++, head = head.Next)
        {
            if(resultNodes[i].Id != 0)
                continue;

            MoveBackwardUntilRandomIsFound(head, ip, ref resultNodes[i]);
        }
    }

    private static void MoveBackwardUntilRandomIsFound(ListNode head, int ip, ref ListNodeJsonModel resultNode)
    {
        ListNode current = head;
        int counter = ip;

        while (current != null)
        {
            counter--;
            current = current.Previous;

            if (!ReferenceEquals(head.Random, current)) 
                continue;

            resultNode.Random = counter;
            resultNode.Id = ip;
            break;
        }
    }

    private ListNodeJsonModel[] ConvertToArrayWithIncompleteRandoms(ListNode head)
    {
        int listSize = this.listSize ?? GetListSize(head);
        var resultArray = new ListNodeJsonModel[listSize];
        var resultModels = new Span<ListNodeJsonModel>(resultArray);

        for (int i = 0; i < listSize; i++)
        {
            resultModels[i] = TryCreateJsonModelWithRandom(head, i + 1);
            head = head.Next;
        }

        return resultArray;
    }

    private static ListNodeJsonModel TryCreateJsonModelWithRandom(ListNode head, int counter)
    {
        var jsonNode = new ListNodeJsonModel();
        jsonNode.Id = counter;
        jsonNode.Data = head.Data;

        if (IsRandomNullCurrentOrPrevious(head, counter, ref jsonNode))
            return jsonNode;

        return TryMoveForwardUntilRandomIsFound(head, counter, ref jsonNode);
    }

    private static ListNodeJsonModel TryMoveForwardUntilRandomIsFound(ListNode head, int counter,
        ref ListNodeJsonModel jsonNode)
    {
        ListNode current = head;

        while (current != null)
        {
            counter++;
            
            if (ReferenceEquals(head.Random, current.Next))
            {
                jsonNode.Random = counter;
                return jsonNode;
            }

            current = current.Next;
        }

        if (!jsonNode.Random.HasValue)
            jsonNode.Id = 0;
        
        return jsonNode;
    }

    private static bool IsRandomNullCurrentOrPrevious(ListNode current, int counter, ref ListNodeJsonModel jsonNode)
    {
        ListNode random = current.Random;
        
        if (random == null)
            return true;
        
        if (ReferenceEquals(random, current))
        {
            jsonNode.Random = counter;
            return true;
        }
        
        if (ReferenceEquals(random, current.Previous))
        {
            jsonNode.Random = counter - 1;
            return true;
        }

        return false;
    }
    
    private static int GetListSize(ListNode head)
    {
        int counter = 0;
        
        while (head != null)
        {
            counter++;
            head = head.Next;
        }

        return counter;
    }
}