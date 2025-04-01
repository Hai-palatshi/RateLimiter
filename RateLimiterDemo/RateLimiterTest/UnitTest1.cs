using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiterDemo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiterTests
{
    [TestClass]
    public class RateLimiterUnitTests
    {
        [TestMethod]
        public async Task SingleCall_Should_Execute_Immediately()
        {
            int counter = 0;

            Func<string, Task> action = arg =>
            {
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            };

            var rules = new List<RateLimitRule>
            {
                new RateLimitRule(1, TimeSpan.FromSeconds(1))
            };

            var limiter = new RateLimiter<string>(action, rules);

            var stopwatch = Stopwatch.StartNew();
            await limiter.Perform("Call 1");
            stopwatch.Stop();

            Assert.AreEqual(1, counter);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, "Call took too long for a single execution");
        }

        [TestMethod]
        public async Task ExceedingLimit_Should_Delay_Call()
        {
            int counter = 0;

            Func<string, Task> action = arg =>
            {
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            };

            var rules = new List<RateLimitRule>
            {
                new RateLimitRule(2, TimeSpan.FromSeconds(1))
            };

            var limiter = new RateLimiter<string>(action, rules);

            await limiter.Perform("Call 1");
            await limiter.Perform("Call 2");

            var stopwatch = Stopwatch.StartNew();
            await limiter.Perform("Call 3");
            stopwatch.Stop();

            Assert.AreEqual(3, counter);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 1000, "Expected delay when limit is exceeded");
        }

        [TestMethod]
        public async Task MultipleCalls_Should_All_Execute_With_Throttle()
        {
            int counter = 0;

            Func<string, Task> action = arg =>
            {
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            };

            var rules = new List<RateLimitRule>
            {
                new RateLimitRule(3, TimeSpan.FromSeconds(1))
            };

            var limiter = new RateLimiter<string>(action, rules);

            var stopwatch = Stopwatch.StartNew();

            var tasks = new List<Task>();
            for (int i = 0; i < 6; i++)
            {
                tasks.Add(limiter.Perform($"Call {i}"));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Assert.AreEqual(6, counter);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 1000);
        }

        [TestMethod]
        public async Task MultipleRules_Should_Enforce_All()
        {
            int counter = 0;

            Func<string, Task> action = arg =>
            {
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            };

            var rules = new List<RateLimitRule>
            {
                new RateLimitRule(5, TimeSpan.FromSeconds(1)),
                new RateLimitRule(10, TimeSpan.FromSeconds(3))
            };

            var limiter = new RateLimiter<string>(action, rules);

            var tasks = new List<Task>();
            for (int i = 0; i < 12; i++)
            {
                tasks.Add(limiter.Perform($"Call {i}"));
            }

            var stopwatch = Stopwatch.StartNew();
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Assert.AreEqual(12, counter);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 2000, "Multiple rules should cause delay");
        }

        [TestMethod]
        public async Task ParallelCalls_Should_Respect_RateLimits()
        {
            int counter = 0;

            Func<string, Task> action = arg =>
            {
                Interlocked.Increment(ref counter);
                return Task.CompletedTask;
            };

            var rules = new List<RateLimitRule>
            {
                new RateLimitRule(2, TimeSpan.FromSeconds(1))
            };

            var limiter = new RateLimiter<string>(action, rules);

            var stopwatch = Stopwatch.StartNew();

            var tasks = new List<Task>();
            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(() => limiter.Perform($"Call {i}")));
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Assert.AreEqual(4, counter);
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= 1000, "Parallel calls should still throttle");
        }
    }
}
