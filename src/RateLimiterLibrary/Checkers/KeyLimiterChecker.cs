using System.Collections.Concurrent;
using RateLimiterLibrary.Contracts;
using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Checkers;

public class KeyLimiterChecker<TKey> : IDisposable where TKey : notnull
{
	private readonly ITimeProvider timeProvider;
	private readonly ManualResetEventSlim resetEvent = new(true);
	private readonly ConcurrentDictionary<TKey, LimiterChecker> limiterCheckers = new();
	private bool disposed;

	public KeyLimiterChecker(ITimeProvider timeProvider, IReadOnlyCollection<Limit<TKey>> limits)
	{
		this.timeProvider = timeProvider;
		foreach (var limit in limits)
		{
			limiterCheckers[limit.Key] = new LimiterChecker(timeProvider, limit.MaxRequestCount, limit.WindowSize);
		}
	}
	
	public CheckResult CanExecuteRequest(TKey key)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(nameof(KeyLimiterChecker<TKey>));
		}
		
		resetEvent.Wait();

		if (!limiterCheckers.TryGetValue(key, out LimiterChecker? checker))
		{
			return CheckResult.NotFound;
		}

		return checker.CanExecuteRequest() ? CheckResult.Granted : CheckResult.Denied;
	}

	public void UpdateKeys(IReadOnlyCollection<Limit<TKey>> newLimits)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(nameof(KeyLimiterChecker<TKey>));
		}
		
		try
		{
			resetEvent.Reset();

			var toDelete = limiterCheckers.Keys.Except(newLimits.Select(x => x.Key));
			foreach (TKey key in toDelete)
			{
				limiterCheckers.TryRemove(key, out _);
			}
		
			foreach (var limit in newLimits)
			{
				if (limiterCheckers.TryGetValue(limit.Key, out var checker))
				{
					checker.UpdateLimits(limit.MaxRequestCount, limit.WindowSize);	
				}
				else
				{
					limiterCheckers.TryAdd(limit.Key, new LimiterChecker(timeProvider, limit.MaxRequestCount, limit.WindowSize));
				}
			}
		}
		finally
		{
			resetEvent.Set();
		}
	}
	
	public void Dispose()
	{
		disposed = true;
		resetEvent.Dispose();
	}
}