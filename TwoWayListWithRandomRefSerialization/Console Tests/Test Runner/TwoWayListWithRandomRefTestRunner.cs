using BenchmarkDotNet.Running;
using Logic_Layer;
using Logic_Layer.Formatters;
using Logic_Layer.Services;
using Logic_Layer.Services.Abstractions;
using Task_Original_Files;
using Utf8Json;
using Utf8Json.Resolvers;

namespace TwoWayListWithRandomRefSerialization.Test_Runner;

public partial class TwoWayListWithRandomRefTestRunner : IExampleRunner
{
    private const string SOURCE_FILE_PATH = "../../../../ListNodeSampleFile.json";
    private const string OUTPUT_FILE_PATH = "../../../../ListNodeOutputFile.json";

    private readonly bool runBenchmarks;
    private IListSerializer serializer;
    private FileStreamOptions readOptions;
    private FileStreamOptions writeOptions;

    public string SourceFilePath { get; set; }
    public string OutputFilePath { get; set; }

    public TwoWayListWithRandomRefTestRunner(bool runBenchmarks)
    {
        this.runBenchmarks = runBenchmarks;
        SetupInnerState();
        SourceFilePath = runBenchmarks ? BENCHMARK_SOURCE_FILE_PATH : SOURCE_FILE_PATH;
        OutputFilePath = runBenchmarks ? BENCHMARK_OUTPUT_FILE_PATH : OUTPUT_FILE_PATH;
    }
    
    public TwoWayListWithRandomRefTestRunner(string sourceFilePath, string outputFilePath, bool runBenchmarks = false)
    {
        this.runBenchmarks = runBenchmarks;
        SetupInnerState();

        if (Path.Exists(sourceFilePath))
            SourceFilePath = sourceFilePath;
        else SourceFilePath = runBenchmarks ? BENCHMARK_SOURCE_FILE_PATH : SOURCE_FILE_PATH;

        if (string.IsNullOrWhiteSpace(outputFilePath))
            OutputFilePath = runBenchmarks ? BENCHMARK_OUTPUT_FILE_PATH : OUTPUT_FILE_PATH;
        else OutputFilePath = outputFilePath;
    }

    public async ValueTask RunExampleAsync()
    {
        if (runBenchmarks)
        {
            BenchmarkRunner.Run<TwoWayListWithRandomRefTestRunner>();
            return;
        }
        
        await RunDeserializeExample();
        await RunSerializeExample();
        RunDeepCopyExample();
    }

    private void SetupInnerState()
    {
        var listNodeFormatter = new ListNodeFormatter();
        var byteFormatter = new ByteFormatter();
        IJsonFormatterResolver resolver = CompositeResolver.Create(listNodeFormatter, byteFormatter);
        CompositeResolver.RegisterAndSetAsDefault(resolver);
        
        serializer = new LucienMariamSerializer();
        readOptions = new FileStreamOptions
        {
            Options = FileOptions.Asynchronous,
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read
        };
        writeOptions = new FileStreamOptions
        {
            Options = FileOptions.Asynchronous,
            Access = FileAccess.Write,
            Mode = FileMode.Truncate,
            Share = FileShare.None ^ FileShare.Delete
        };
    }

    private async Task RunSerializeExample()
    {
        await using var fileStream = new FileStream(OUTPUT_FILE_PATH, writeOptions);
        ListNode head = ListNodeCreator.CreateSmallProblematicList();
        
        Console.WriteLine("\n=========================");
        Console.WriteLine("=== SERIALIZE EXAMPLE ===  (DONE IN FILE)");
        Console.WriteLine("=========================");
        
        await serializer.Serialize(head, fileStream);
    }

    private async Task RunDeserializeExample()
    {
        int counter = 0;
        await using var fileStream = new FileStream(SOURCE_FILE_PATH, readOptions);
        ListNode head = await serializer.Deserialize(fileStream);
        
        Console.WriteLine("\n=== DESERIALIZE EXAMPLE ===");
        while (head != null)
        {
            Console.WriteLine(head.Data);
            Console.WriteLine(head.Random == null ? "null" : head.Random.Data);
            Console.WriteLine('\n');
            head = head.Next;
            counter++;
        }
        Console.WriteLine("\nNodes number: {0}", counter);
    }

    private void RunDeepCopyExample()
    {
        ListNode head = ListNodeCreator.CreateSmallGeneralList();

        Console.WriteLine("\n\n=== DEEP COPY EXAMPLE ===");
        ListNode newList = serializer.DeepCopy(head).GetAwaiter().GetResult();
        head = null;
        GC.Collect(0, GCCollectionMode.Forced, false, false);
        int counter = 0;
        
        while (newList != null)
        {
            Console.WriteLine(newList.Data);
            Console.WriteLine(newList.Random == null ? "null" : newList.Random.Data);
            Console.WriteLine('\n');
            newList = newList.Next;
            counter++;
        }
        Console.WriteLine("\nNodes number: {0}", counter);
    }
}