// See https://aka.ms/new-console-template for more information

using Logic_Layer.Services.Abstractions;
using TwoWayListWithRandomRefSerialization.Test_Runner;

bool runBenchmarks;

#if DEBUG
Console.WriteLine("[Mode] = [DEBUG]");
runBenchmarks = false;
#else
    Console.WriteLine("[Mode] = [RELEASE]"); 
    runBenchmarks = true;
#endif

IExampleRunner runner = new TwoWayListWithRandomRefTestRunner(runBenchmarks);
await runner.RunExampleAsync();