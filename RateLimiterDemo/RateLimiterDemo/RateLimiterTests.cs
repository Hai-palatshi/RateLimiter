using RateLimiterDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public class RateLimiterTests
{
    private readonly RateLimiter<string> _limiter;

    public RateLimiterTests()
    {
        var rules = new List<RateLimitRule>
        {
            new RateLimitRule(3, TimeSpan.FromSeconds(1)),
            new RateLimitRule(10, TimeSpan.FromSeconds(5))
        };

        
        Func<string, Task> fakeApiCall = async (arg) =>
        {
            Console.WriteLine($"[ {DateTime.Now:HH:mm:ss.fff} ] Calling fake API with: {arg}");
            await Task.Delay(200); 
        };

        _limiter = new RateLimiter<string>(fakeApiCall, rules);
    }

    public async Task RunTestAsync()
    {
        var tasks = new List<Task>();

        for (int i = 1; i <= 15; i++)
        {
            int copy = i;
            tasks.Add(Task.Run(() => _limiter.Perform($"Request {copy}")));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("API Simulation Test Completed.");
    }
}
