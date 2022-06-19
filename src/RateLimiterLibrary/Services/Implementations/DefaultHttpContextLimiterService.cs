using Microsoft.AspNetCore.Http;
using RateLimiterLibrary.Checkers;
using RateLimiterLibrary.Contracts;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Services.Implementations;

public class DefaultHttpContextLimiterService: IHttpContextLimiterService
{
	private readonly LimiterChecker checker;

	public DefaultHttpContextLimiterService(ITimeProvider timeProvider, IConfigProvider<DefaultLimiterOptions> options)
	{
		DefaultLimiterOptions currentConfig = options.GetCurrentConfig();
		checker = new LimiterChecker(timeProvider, currentConfig.RequestsLimit, currentConfig.WindowSize);

		options.OnNewConfig += OptionsChanged;
	}
	
	public CheckResult CanExecuteRequest(HttpContext httpContext)
	{
		return checker.CanExecuteRequest() ? CheckResult.Granted : CheckResult.Denied;
	}

	private void OptionsChanged(DefaultLimiterOptions options)
	{
		checker.UpdateLimits(options.RequestsLimit, options.WindowSize);
	}
}