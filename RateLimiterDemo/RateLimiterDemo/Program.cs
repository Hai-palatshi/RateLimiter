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

        var rules2 = new List<RateLimitRule>
        {
            new RateLimitRule(10, TimeSpan.FromSeconds(1))
        };

        // create RateLimiter
        var limiter = new RateLimiter<string>(exampleAction, rules);
        var limiter2 = new RateLimiter<string>(exampleAction, rules2);

        // create call
        var tasks = new List<Task>();
        for (int i = 1; i <= 10; i++)
        {
            int copy = i;
            tasks.Add(limiter.Perform($"Call {copy}"));
        }

        for (int i = 1; i <= 10; i++)
        {
            int copy = i;
            tasks.Add(limiter2.Perform($"Call {copy}"));
        }

        await Task.WhenAll(tasks);



        // בדיקה אמיתית של קריאות ל-"API"
        //var testRunner = new RateLimiterTests();
        //await testRunner.RunTestAsync();


        Console.WriteLine("All calls finished.");
    }
}
