using RateLimiterLibrary.Providers;

namespace RateLimiterLibraryTests.Helpers;

public class TestTimeProvider : ITimeProvider
{
	private DateTime dateTime = DateTime.UtcNow;

	public long GetCurrentTicks()
	{
		return dateTime.Ticks;
	}

	public void Add(TimeSpan timeSpan)
	{
		dateTime += timeSpan;
	}
}