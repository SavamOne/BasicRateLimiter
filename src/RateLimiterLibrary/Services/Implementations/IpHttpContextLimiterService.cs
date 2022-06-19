using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RateLimiterLibrary.Contracts;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Providers;

namespace RateLimiterLibrary.Services.Implementations;

public class IpHttpContextLimiterService : BaseHttpContextLimiterService<IpLimiterOptions, IPAddress>
{
	public IpHttpContextLimiterService(ITimeProvider timeProvider, IConfigProvider<IpLimiterOptions> userLimiterOptions)
	: base(timeProvider, userLimiterOptions)
	{
	}

	protected override IPAddress GetKeyFromHttpContext(HttpContext context)
	{
		return context.Connection.RemoteIpAddress!;
	}
	
	protected override IReadOnlyCollection<Limit<IPAddress>> Convert(IpLimiterOptions options)
	{
		return options.Options.Select(Convert).ToList();
	}
	
	private static Limit<IPAddress> Convert(IpLimiterOption option)
	{
		return new Limit<IPAddress>(option.IpAddress, option.RequestsLimit, option.WindowSize);
	}
}