namespace Logic_Layer.Services;

public class RetryProvider
{
    private const ushort MAX_ATTEMPTS_NUM = 3;
    private const ushort WAIT_INTERVAL_INCREASE_RATE = 5;
    private const ushort WAIT_INTERVAL_IN_MILLI_SECS = 1000;
    
    private readonly ushort maxAttemptsNum;
    private readonly ushort waitIntervalIncreaseRate;
    
    private int waitIntervalInMilliSecs;

    public RetryProvider(int waitIntervalInMilliSecs = WAIT_INTERVAL_IN_MILLI_SECS,
        ushort maxAttemptsNum = MAX_ATTEMPTS_NUM, ushort waitIntervalIncreaseRate = WAIT_INTERVAL_INCREASE_RATE)
    {
        this.maxAttemptsNum = maxAttemptsNum;
        this.waitIntervalIncreaseRate = waitIntervalIncreaseRate;
        this.waitIntervalInMilliSecs = waitIntervalInMilliSecs;
    }

    public async Task RetryStreamRead(Stream s)
    {
        for (ushort i = 0; i < maxAttemptsNum; i++)
        {
            Console.WriteLine($"File read retry -- [{i + 1}].");
            await Task.Delay(waitIntervalInMilliSecs);
            
            if(s.CanRead) 
                return;
            
            waitIntervalInMilliSecs *= waitIntervalIncreaseRate;
        }

        throw new TimeoutException("Stream read timeout error.");
    }
    
    public async Task RetryStreamWrite(Stream s)
    {
        for (ushort i = 0; i < maxAttemptsNum; i++)
        {
            Console.WriteLine($"File write retry -- [{i + 1}].");
            await Task.Delay(waitIntervalInMilliSecs);
            
            if(s.CanWrite) 
                return;
            
            waitIntervalInMilliSecs *= waitIntervalIncreaseRate;
        }

        throw new TimeoutException("Stream write timeout error.");
    }
    
    
}