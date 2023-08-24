using Logic_Layer.Models;
using Logic_Layer.Services.Abstractions;
using Logic_Layer.Strategies.Abstractions;
using Task_Original_Files;

namespace Logic_Layer.Strategies;

/// <summary>
/// Universal algorithm for both short and long lists
/// </summary>
/// <remarks>
/// Big O = 2n(1 + n)
/// 4 cycles: n + n*n + n
/// </remarks>
public class SerializeListNode4CyclesWithParallelStrategy : SerializeListNodeStrategy, ISerializeStrategy<ListNode>
{
    public const int MIN_EFFECTIVE_LIST_SIZE = 750;
    
    private readonly ParallelOptions options;

    public SerializeListNode4CyclesWithParallelStrategy()
    {
        options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 3
        };
    }
    
    public override async Task HandleList(ListNode head, Stream s)
    {
        Dictionary<int, ListNode> dictionary = ConvertToDictionary(head);
        int listSize = dictionary.Count;

        if (dictionary.Count < MIN_EFFECTIVE_LIST_SIZE)
        {
            var otherAlgorithm = new SerializeListNode6CyclesSyncStrategy(listSize);
            await otherAlgorithm.HandleList(head, s);
            return;
        }
        
        ListNodeJsonModel[] jsonModels = await ConvertToJsonModelsArray(dictionary, head);
        await this.Serialize(jsonModels, s);
    }

    private async Task<ListNodeJsonModel[]> ConvertToJsonModelsArray(Dictionary<int, ListNode> dictionary,
        ListNode head)
    {
        var jsonModelsArray = new ListNodeJsonModel[dictionary.Count];
        await FillInJsonModels(jsonModelsArray, dictionary);
        return jsonModelsArray;
    }

    private Task FillInJsonModels(ListNodeJsonModel[] jsonModels, Dictionary<int, ListNode> dictionary)
    {
        return Parallel.ForEachAsync(dictionary, options, async (keyValue, token) =>
        {
            int ip = keyValue.Key;
            int i = ip - 1;
            ListNode current = keyValue.Value;
            ListNode random = current.Random;
            jsonModels[i] = CreateNode(current, ip);
        
            if (random == null || ReferenceEquals(random, current))
            {
                jsonModels[i].Random = random == null ? null : ip;
                return;
            }
        
            jsonModels[i].Random = await FindRandom(random, current, ip);
        });
    }

    private static async Task<int> FindRandom(ListNode random, ListNode current, int id)
    {
        int randomId = -1;
        using var tokenSource = new CancellationTokenSource();
        var ct = tokenSource.Token;
        var taskFactory = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.None);
        Task<int> moveLeft = taskFactory.StartNew(() => MoveLeftUntilRandomIsFound(random, current, id), ct);
        Task<int> moveRight = taskFactory.StartNew(() => MoveRightUntilRandomIsFound(random, current, id), ct);
        
        var taskList = new List<Task<int>>(2);
        taskList.Add(moveLeft);
        taskList.Add(moveRight);

        while (taskList.Any())
        {
            Task<int> finishedTask = await Task.WhenAny(taskList);
            taskList.Remove(finishedTask);
            randomId = await finishedTask;

            if (randomId == -1)
                continue;
                
            tokenSource.Cancel(false);
            break;
        }
        
        return randomId;
    }

    private static int MoveLeftUntilRandomIsFound(ListNode random, ListNode current, int id)
    {
        current = current.Previous;

        while (current != null)
        {
            id--;

            if (ReferenceEquals(random, current))
                return id;
            
            current = current.Previous;
        }

        return -1;
    }
    
    private static int MoveRightUntilRandomIsFound(ListNode random, ListNode current, int id)
    {
        current = current.Next;

        while (current != null)
        {
            id++;

            if (ReferenceEquals(random, current))
                return id;
            
            current = current.Next;
        }

        return -1;
    }

    private static Dictionary<int, ListNode> ConvertToDictionary(ListNode head)
    {
        int counter = 0;
        var dictionary = new Dictionary<int, ListNode>();
        
        while (head != null)
        {
            counter++;
            dictionary.Add(counter, head);
            
            head = head.Next;
        }

        return dictionary;
    }

    private static ListNodeJsonModel CreateNode(ListNode head, int id)
    {
        return new ListNodeJsonModel
        {
            Id = id,
            Data = head.Data
        };
    }
}