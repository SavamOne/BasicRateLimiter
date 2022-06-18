using System.Diagnostics;

namespace RateLimiterLibrary.Providers.Implementations;

public class StopwatchTicksProvider : ITimeProvider
{
	public long GetCurrentTicks()
	{
		return Stopwatch.GetTimestamp();
	}
}