using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Checkers;

public class LimiterChecker
{
	private readonly ITimeProvider timeProvider;

	private long currentWindowStartTicks;
	private int currentWindowRequestCount;

	private int windowMaxRequestCount;
	private long windowSizeInTicks;

	private readonly object locker = new();

	public LimiterChecker(ITimeProvider timeProvider, int windowMaxRequestsCount, TimeSpan windowSize)
	{
		this.timeProvider = timeProvider;
		UpdateLimits(windowMaxRequestsCount, windowSize);
	}
	
	public bool CanExecuteRequest()
	{
		if (Interlocked.CompareExchange(ref windowMaxRequestCount, windowMaxRequestCount, 0) <= 0)
		{
			return true;
		}
		
		long currentTicks = timeProvider.GetCurrentTicks();
		
		lock (locker)
		{
			if (currentTicks >= currentWindowStartTicks + windowSizeInTicks)
			{
				currentWindowStartTicks = currentTicks;
				currentWindowRequestCount = 1;
				return true;
			}

			if (currentWindowRequestCount >= windowMaxRequestCount)
			{
				return false;
			}
			
			currentWindowRequestCount++;
			return true;

		}
	}

	public void UpdateLimits(int newMaxRequestCount, TimeSpan windowSize)
	{
		if (newMaxRequestCount > 0 && windowSize <= TimeSpan.Zero)
		{
			throw new ArgumentException(nameof(windowSize));
		}
		
		lock (locker)
		{
			windowMaxRequestCount = newMaxRequestCount;
			windowSizeInTicks = windowSize.Ticks;
		}
	}
}