using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiterLibrary.Checkers;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Services.Implementations;

public class DefaultHttpContextLimiterService: IHttpContextLimiterService
{
	private readonly LimiterChecker checker;
	private readonly IDisposable changeTracker;

	public DefaultHttpContextLimiterService(ITimeProvider timeProvider, IOptionsMonitor<DefaultLimiterOptions> options)
	{
		checker = new LimiterChecker(timeProvider, options.CurrentValue.RequestsLimit, options.CurrentValue.WindowSize);
		
		changeTracker = options.OnChange(OptionsChanged);
	}
	
	public CheckResult CanExecuteRequest(HttpContext httpContext)
	{
		return checker.CanExecuteRequest() ? CheckResult.Granted : CheckResult.Denied;
	}

	private void OptionsChanged(DefaultLimiterOptions options)
	{
		checker.UpdateLimits(options.RequestsLimit, options.WindowSize);
	}
	
	public void Dispose()
	{
		changeTracker.Dispose();
	}
}