using BenchmarkDotNet.Attributes;
using Logic_Layer;
using Logic_Layer.Services;
using Logic_Layer.Strategies;
using Task_Original_Files;

// ReSharper disable once CheckNamespace
namespace TwoWayListWithRandomRefSerialization.Test_Runner;

public partial class TwoWayListWithRandomRefTestRunner
{
    private const string BENCHMARK_SOURCE_FILE_PATH = "../../../../../../../../ListNodeSampleFile.json";
    private const string BENCHMARK_OUTPUT_FILE_PATH = "../../../../../../../../ListNodeOutputFile.json";

    private ListNode list;
    private Stream serializeWriteStream;
    private Stream deserializeReadStream;

    [Params(13, 100, 1000, 10000, 100000)]
    public int ListSize;
    
    /// <summary>
    /// BENCHMARK ONLY CONSTRUCTOR
    /// </summary>
    public TwoWayListWithRandomRefTestRunner() : this(true)
    {
        ((LucienMariamSerializer)this.serializer).DeserializeAlgorithm = new DeserializeListNodeStrategy(false);
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        list = ListNodeCreator.CreateList(ListSize);
    }
    
    [GlobalSetup(Target = nameof(MeasureDeserialize))]
    public async Task DeserializeGlobalSetup()
    {
        list = ListNodeCreator.CreateList(ListSize);
        Stream deserializeWriteStream = new FileStream(OutputFilePath, writeOptions);
        await this.serializer.Serialize(list, deserializeWriteStream);
        await deserializeWriteStream.DisposeAsync();
    }
    
    [IterationSetup(Target = nameof(MeasureSerialize))]
    public void SerializeIterationSetup()
    {
        serializeWriteStream = new FileStream(OutputFilePath, writeOptions);
    }

    [IterationSetup(Target = nameof(MeasureDeserialize))]
    public void DeserializeIterationSetup()
    {
        deserializeReadStream = new FileStream(OutputFilePath, readOptions);
    }
    
    [Benchmark]
    public async Task MeasureSerialize()
    {
        await this.serializer.Serialize(list, serializeWriteStream);
    }

    [Benchmark]
    public async Task<ListNode> MeasureDeserialize()
    {
        return await this.serializer.Deserialize(deserializeReadStream);
    }

    [Benchmark]
    public ListNode MeasureDeepCopy()
    {
        return this.serializer.DeepCopy(list).GetAwaiter().GetResult();
    }

    [IterationCleanup(Target = nameof(MeasureSerialize))]
    public void SerializeIterationCleanup()
    {
        serializeWriteStream.Dispose();
    }
    
    [IterationCleanup(Target = nameof(MeasureDeserialize))]
    public void DeserializeIterationCleanup()
    {
        deserializeReadStream.Dispose();
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        GC.Collect(0, GCCollectionMode.Optimized, false, false);
    }
}