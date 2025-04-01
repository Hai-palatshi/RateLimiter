using RateLimiterDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Func<string, Task> exampleAction =  arg =>
        {
            Console.WriteLine($"[ {DateTime.Now:HH:mm:ss.fff} ] Executing: {arg}");
            return Task.CompletedTask;
        };

        var rules = new List<RateLimitRule>
        {
            new RateLimitRule(3, TimeSpan.FromSeconds(1))
        };

        // create RateLimiter
        var limiter = new RateLimiter<string>(exampleAction, rules);

        // create call
        var tasks = new List<Task>();
        for (int i = 1; i <= 10; i++)
        {
            int copy = i;
            tasks.Add(limiter.Perform($"Call {copy}"));
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("All calls finished.");
    }
}
