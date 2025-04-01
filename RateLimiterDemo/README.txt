Explanation of the Two Rate Limiting Approaches
Fixed (Absolute) Window:
Limits the number of allowed calls in a fixed time window. For example: up to 100 calls per full hour. The count resets at the beginning of each window, like from 00:00 to 01:00, from 01:00 to 02:00, and so on.

Sliding Window:
Checks the number of calls dynamically based on the actual time they were made. For example: has there been more than 100 calls in the last 60 minutes — at any given moment, not just by full hour.

Why I chose Sliding Window
I chose the Sliding Window approach because:
- It provides more accurate and consistent rate limiting, regardless of the system clock.
- It helps prevent sudden traffic spikes, which can happen in Fixed Window logic.

Even though Sliding Window requires a bit more code to implement, the accuracy and stability it offers make it the better choice 
especially in systems that handle high traffic or work with multiple threads.