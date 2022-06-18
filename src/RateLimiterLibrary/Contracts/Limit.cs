namespace RateLimiterLibrary.Contracts;

public class Limit<T>
{
	public Limit(T key, int maxRequestCount, TimeSpan windowSize)
	{
		Key = key;
		MaxRequestCount = maxRequestCount;
		WindowSize = windowSize;
	}

	public T Key { get; }
	
	public int MaxRequestCount { get; set; }
	
	public TimeSpan WindowSize { get; set; }
}