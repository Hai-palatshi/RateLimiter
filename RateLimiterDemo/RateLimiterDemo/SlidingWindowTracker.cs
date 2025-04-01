using System;
using System.Collections.Generic;

public class SlidingWindowTracker
{
    private int maxCalls;
    private TimeSpan timeWindow;
    private Queue<DateTime> callTimes;
    private object locker;

    public SlidingWindowTracker(int limit, TimeSpan window)
    {
        maxCalls = limit;
        timeWindow = window;
        callTimes = new Queue<DateTime>();
        locker = new object();
    }
    public bool CanExecute()
    {
        lock (locker)
        {
            DateTime now = DateTime.UtcNow;

            //We will clean up old calls that went out of the time window.
            while (callTimes.Count > 0)
            {
                DateTime oldestCallTime = callTimes.Peek();
                TimeSpan timeSinceOldestCall = now - oldestCallTime;

                if (timeSinceOldestCall > timeWindow)
                {
                    callTimes.Dequeue(); // Removing an old call from the queue
                }
                else
                {
                    break; // All other readings are new.
                }
            }
            // check if a new call is allowed.
            return callTimes.Count < maxCalls;
        }
    }
    public void RecordExecution()
    {
        lock (locker)
        {
            callTimes.Enqueue(DateTime.UtcNow);
        }
    }
}
