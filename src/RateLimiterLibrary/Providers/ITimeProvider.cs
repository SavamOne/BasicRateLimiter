namespace RateLimiterLibrary.Providers;

public interface ITimeProvider
{
	long GetCurrentTicks();
}