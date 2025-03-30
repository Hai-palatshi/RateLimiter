using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterDemo
{
    public class RateLimitRule
    {
        public int Limit { get; }
        public TimeSpan Window { get; }

        public RateLimitRule(int limit, TimeSpan window)
        {
            Limit = limit;
            Window = window;
        }
    }
}
