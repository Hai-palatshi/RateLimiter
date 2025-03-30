using RateLimiterDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RateLimiter<TArg>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private Func<TArg, Task> _action; // The function to execute
    private List<SlidingWindowTracker> _rules; // List of rules (limits)

    public RateLimiter(Func<TArg, Task> action, IEnumerable<RateLimitRule> rulesList)
    {
        _action = action;
        _rules = new List<SlidingWindowTracker>();

        // Create a tracker for each rate limit rule
        foreach (var rule in rulesList)
        {
            var tracker = new SlidingWindowTracker(rule.Limit, rule.Window);
            _rules.Add(tracker);
        }
    }

    public async Task Perform(TArg input)
    {

        bool printedWaiting = false;

        // Keep checking if the action is allowed based on all rules
        while (true)
        {
            await _semaphore.WaitAsync();
            bool isAllowed = true;

            foreach (var rule in _rules)
            {
                if (!rule.CanExecute())
                {
                    isAllowed = false;
                    break;
                }
            }

            if (isAllowed)
            {
                // Record execution for each rule
                foreach (var rule in _rules)
                {
                    rule.RecordExecution();
                }
                _semaphore.Release();
                break; // Now allowed to continue
            }
            _semaphore.Release();
            if (!printedWaiting)
            {
                Console.WriteLine($"⏳ [{DateTime.Now:HH:mm:ss.fff}] Waiting for: {input}");
                printedWaiting = true;
            }
            // Wait a bit before trying again
            await Task.Delay(100);
        }

        // Execute the actual action
        await _action(input);
    }
}
