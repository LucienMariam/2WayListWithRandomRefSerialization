using Logic_Layer.Models;
using Logic_Layer.Services;
using Logic_Layer.Services.Abstractions;
using Task_Original_Files;
using Utf8Json;

namespace Logic_Layer.Strategies;

public class DeserializeListNodeStrategy : IDeserializeStrategy<ListNode>
{
    private const byte MIN_NODE_SIZE = 30;

    private readonly bool isCachingOn;
    private readonly InMemoryCacheService cacheService;

    public DeserializeListNodeStrategy(bool turnOnCaching = true)
    {
        isCachingOn = turnOnCaching;
        cacheService = new InMemoryCacheService(20);
    }

    public async ValueTask<ListNode> Deserialize(Stream s)
    {
        ListNode listNode = null;
        string cacheKey = null;

        if (isCachingOn)
        {
            listNode = GetFromCache(s, out string cacheKeyOutput);
            cacheKey = cacheKeyOutput;
        }

        if (listNode != null)
            return listNode;

        int advancedNodesNumber = checked((int)(s.Length / MIN_NODE_SIZE));
        byte[] streamInBytes = await StreamConverter.ToByteArray(s);
        var reader = new JsonReader(streamInBytes);

        while (!reader.ReadIsBeginArray())
            reader.ReadNext();

        listNode = DeserializeFromJsonView(ref reader, advancedNodesNumber);
        
        if(isCachingOn)
            cacheService.Set(cacheKey, listNode);
        
        return listNode;
    }

    private ListNode GetFromCache(Stream s, out string cacheKey)
    {
        cacheKey = null;
        var fileStream = s as FileStream;
        string fileName = fileStream?.Name;

        if (!string.IsNullOrEmpty(fileName))
            cacheKey = $"{fileName}-{fileStream.Length}";

        return string.IsNullOrEmpty(cacheKey) ? null : cacheService.Get<ListNode>(cacheKey);
    }
    
    private ListNode DeserializeFromJsonView(ref JsonReader reader, int advancedNodesNumber)
    {
        var listNodeArray = new ListNode[advancedNodesNumber];
        var jsonViewArray = new ListNodeJsonModel[advancedNodesNumber - 1];
        var resultSet = new Span<ListNode>(listNodeArray);
        var tempBuffer = new Span<ListNodeJsonModel>(jsonViewArray);
        int position = HandleFirstElement(ref reader, ref resultSet, ref tempBuffer);

        for (int i = 1, im = i - 1, ip = i + 1; !IsLastNodeRead(ref reader); i++, im++, ip++)
        {
            tempBuffer[position] = JsonSerializer.Deserialize<ListNodeJsonModel>(ref reader);
            resultSet[i] = new ListNode();
            resultSet[i].Data = tempBuffer[position].Data;
            resultSet[i].Previous = resultSet[im];
            resultSet[im].Next = resultSet[i];
            
            int? random = tempBuffer[position].Random;
            
            if (random.HasValue && random > ip)
                tempBuffer[position++].Id = ip;
            else resultSet[i].Random = random.HasValue ? resultSet[random.Value - 1] : null;
        }

        RefineBuffers(ref resultSet, ref tempBuffer);
        FillInMissedRandoms(ref resultSet, ref tempBuffer);
        return resultSet[0];
    }

    private bool IsLastNodeRead(ref JsonReader reader) => reader.ReadIsEndObject() && reader.ReadIsEndArray();

    private ushort HandleFirstElement(ref JsonReader reader, ref Span<ListNode> resultSet,
        ref Span<ListNodeJsonModel> tempBuffer)
    {
        ushort position = 0;
        tempBuffer[0] = JsonSerializer.Deserialize<ListNodeJsonModel>(ref reader);
        resultSet[0] = new ListNode();
        resultSet[0].Data = tempBuffer[0].Data;
        int? random = tempBuffer[0].Random;
        
        if (random.HasValue && random != 1 && random != 0)
        {
            tempBuffer[0].Id = 1;
            position++;
        }
        else resultSet[0].Random = random.HasValue ? resultSet[0] : null;

        return position;
    }

    private void RefineBuffers(ref Span<ListNode> resultSet, ref Span<ListNodeJsonModel> tempBuffer)
    {
        int resultSetEmptyElementsNum = 0;
        int tempBufferEmptyElementsNum = 0;

        for (int i = resultSet.Length - 1; resultSet[i] == null; i--)
            resultSetEmptyElementsNum++;
        
        for (int i = tempBuffer.Length - 1; tempBuffer[i].Id == 0; i--)
            tempBufferEmptyElementsNum++;

        if (resultSetEmptyElementsNum > 0)
        {
            int payloadLength = resultSet.Length - resultSetEmptyElementsNum;
            resultSet = resultSet[..payloadLength];
        }

        if (tempBufferEmptyElementsNum > 0)
        {
            int payloadLength = tempBuffer.Length - tempBufferEmptyElementsNum;
            tempBuffer = tempBuffer[..payloadLength];
        }
    }

    private void FillInMissedRandoms(ref Span<ListNode> resultSet, ref Span<ListNodeJsonModel> tempBuffer)
    {
        int nodeId;
        int randomId;

        for (int i = 0; i < tempBuffer.Length; i++)
        {
            nodeId = tempBuffer[i].Id - 1;
            randomId = tempBuffer[i].Random.Value - 1;
            resultSet[nodeId].Random = resultSet[randomId];
        }
    }
}