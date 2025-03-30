using RateLimiterDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

public class RateLimiterTests
{
    [Fact]
    public async Task RateLimiter_EnforcesLimit()
    {
        // Arrange
        var results = new List<DateTime>();
        var rules = new List<RateLimitRule>
        {
            new RateLimitRule(3, TimeSpan.FromSeconds(1))
        };

        var limiter = new RateLimiter<string>(async (arg) =>
        {
            results.Add(DateTime.UtcNow);
            await Task.Delay(10); // simulate API delay
        }, rules);

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < 6; i++)
        {
            tasks.Add(limiter.Perform($"Test {i}"));
        }

        await Task.WhenAll(tasks);

        // Assert: נבדוק שהזמנים מראים על הפרדה של לפחות שנייה בין 3 הקריאות הראשונות ל-3 האחרונות
        var firstBatch = results.GetRange(0, 3);
        var secondBatch = results.GetRange(3, 3);

        double gap = (secondBatch[0] - firstBatch[2]).TotalMilliseconds;

        Assert.True(gap >= 900, $"Expected gap >= 900ms but got {gap}ms");
    }
}
