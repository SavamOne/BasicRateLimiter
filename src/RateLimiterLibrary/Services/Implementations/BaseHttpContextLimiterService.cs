using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiterLibrary.Checkers;
using RateLimiterLibrary.Contracts;
using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Services.Implementations;

public abstract class BaseHttpContextLimiterService<TOptions, TKey> : IHttpContextLimiterService
	where TKey : notnull
{
	private readonly KeyLimiterChecker<TKey> limiterChecker;
	private readonly IDisposable changeTracker;
	private bool disposed;

	protected BaseHttpContextLimiterService(ITimeProvider timeProvider, IOptionsMonitor<TOptions> options)
	{
		limiterChecker = new KeyLimiterChecker<TKey>(timeProvider, Convert(options.CurrentValue));

		changeTracker = options.OnChange(OptionsChanged);
	}

	public CheckResult CanExecuteRequest(HttpContext httpContext)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(nameof(BaseHttpContextLimiterService<TOptions, TKey>));
		}
		
		return limiterChecker.CanExecuteRequest(GetKeyFromHttpContext(httpContext));
	}

	protected abstract TKey GetKeyFromHttpContext(HttpContext context);

	protected abstract IReadOnlyCollection<Limit<TKey>> Convert(TOptions options);

	private void OptionsChanged(TOptions newOptions)
	{
		limiterChecker.UpdateKeys(Convert(newOptions));
	}

	public void Dispose()
	{
		changeTracker.Dispose();
		limiterChecker.Dispose();
		disposed = true;
	}
}