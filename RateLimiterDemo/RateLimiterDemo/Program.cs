using RateLimiterDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // פעולה לדוגמה – תדפיס את שם הקריאה והשעה
        Func<string, Task> exampleAction = async (arg) =>
        {
            Console.WriteLine($"[ {DateTime.Now:HH:mm:ss.fff} ] Executing: {arg}");
            await Task.Delay(50); // מדמה פעולה אמיתית (למשל קריאת API)
        };

        // הגדרת מגבלות קצב – עד 3 קריאות בכל שנייה
        var rules = new List<RateLimitRule>
        {
            new RateLimitRule(3, TimeSpan.FromSeconds(1))
        };

        // יצירת RateLimiter
        var limiter = new RateLimiter<string>(exampleAction, rules);

        // יצירת 10 קריאות במקביל
        var tasks = new List<Task>();
        for (int i = 1; i <= 10; i++)
        {
            int copy = i; // כדי לשמור את הערך הנכון
            tasks.Add(  limiter.Perform($"Call {copy}"));
        }

        // מחכים שכל הקריאות יסתיימו
        await Task.WhenAll(tasks);



        // בדיקה אמיתית של קריאות ל-"API"
        var testRunner = new RateLimiterTests();
        await testRunner.RunTestAsync();


        Console.WriteLine("✅ All calls finished.");
    }
}
