using Logic_Layer.Services.Abstractions;
using Logic_Layer.Strategies.Abstractions;
using Task_Original_Files;

namespace Logic_Layer.Strategies;

public class DeepCopyListNodeArrayStrategy : DeepCopyListNodeStrategy, IDeepCopyStrategy<ListNode>
{
    private readonly ParallelOptions options;

    public DeepCopyListNodeArrayStrategy()
    {
        options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
    }

    public override ListNode DeepCopy(ListNode copyFrom)
    {
        if (copyFrom.Next == null)
            return this.DeepCopyFirstElement(copyFrom);

        int listSize = GetListSize(copyFrom);
        var resultArray = new ListNode[listSize];
        var tempArray = new ListNode[listSize];
        Initialize(resultArray, tempArray, copyFrom);
        FillInRandoms(resultArray, tempArray);
        tempArray = null;
        return resultArray[0];
    }

    private void FillInRandoms(ListNode[] resultArray, ListNode[] tempArray)
    {
        Parallel.ForEach(resultArray, options, (node, state, id) =>
        {
            ListNode random = tempArray[id].Random;
            
            if(random == null)
                return;
            
            var buffer = new Span<ListNode>(tempArray);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (!ReferenceEquals(random, buffer[i])) 
                    continue;
                
                node.Random = resultArray[i];
                return;
            }
        });
    }
    
    private int GetListSize(ListNode head)
    {
        int listSize = 0;

        while (head != null)
        {
            listSize++;
            head = head.Next;
        }

        return listSize;
    }

    private void Initialize(ListNode[] resultArray, ListNode[] bufferArray, ListNode head)
    {
        var result = new Span<ListNode>(resultArray);
        var buffer = new Span<ListNode>(bufferArray);
        buffer[0] = head;
        result[0] = new ListNode();
        result[0].Data = head.Data;
        head = head.Next;
        
        for (int i = 1; i < result.Length; i++, head = head.Next)
        {
            buffer[i] = head;
            result[i] = new ListNode();
            result[i].Data = head.Data;
            result[i].Previous = result[i - 1];
            result[i - 1].Next = result[i];
        }
    }
}