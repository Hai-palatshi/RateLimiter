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

    // האם מותר לבצע פעולה עכשיו?
    public bool CanExecute()
    {
        lock (locker)
        {
            DateTime now = DateTime.UtcNow;

            // מסיר את כל הקריאות שנעשו מחוץ לחלון הזמן
            while (callTimes.Count > 0 && now - callTimes.Peek() > timeWindow)
            {
                callTimes.Dequeue();
            }

            // אם כמות הקריאות בתוך החלון קטנה מהמגבלה – מותר להריץ
            return callTimes.Count < maxCalls;
        }
    }

    // רושם שבוצעה פעולה עכשיו
    public void RecordExecution()
    {
        lock (locker)
        {
            callTimes.Enqueue(DateTime.UtcNow);
        }
    }
}
