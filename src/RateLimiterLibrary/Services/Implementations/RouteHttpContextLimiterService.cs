using Microsoft.AspNetCore.Http;
using RateLimiterLibrary.Contracts;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Services.Implementations;

public class RouteHttpContextLimiterService : BaseHttpContextLimiterService<RouteLimiterOptions, PathString>
{
	// TODO такую же штуку можно было делать на атрибутах
	public RouteHttpContextLimiterService(ITimeProvider timeProvider, IConfigProvider<RouteLimiterOptions> routeLimiterOptions)
	: base(timeProvider, routeLimiterOptions)
	{
	}

	protected override PathString GetKeyFromHttpContext(HttpContext context)
	{
		return context.Request.Path;
	}
	
	protected override IReadOnlyCollection<Limit<PathString>> Convert(RouteLimiterOptions options)
	{
		return options.Options.Select(Convert).ToList();
	}

	private static Limit<PathString> Convert(RouteLimiterOption routeLimiterOption)
	{
		return new Limit<PathString>(routeLimiterOption.Path, routeLimiterOption.RequestsLimit, routeLimiterOption.WindowSize);
	}
}