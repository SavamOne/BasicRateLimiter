using Microsoft.AspNetCore.Http;
using RateLimiterLibrary.Contracts;

namespace RateLimiterLibrary.Services.Implementations;

public class HttpContextLimiterServiceAggregator : IHttpContextLimiterServiceAggregator
{
	private readonly IEnumerable<IHttpContextLimiterService> limiterServices;
	
	public HttpContextLimiterServiceAggregator(IEnumerable<IHttpContextLimiterService> limiterServices)
	{
		this.limiterServices = limiterServices;
	}
	
	public bool CanExecuteRequest(HttpContext httpContext)
	{
		foreach (IHttpContextLimiterService limiterService in limiterServices)
		{
			CheckResult result = limiterService.CanExecuteRequest(httpContext);
			  
			switch (result)
			{
				case CheckResult.Granted:
					return true;
				case CheckResult.Denied:
					return false;
				case CheckResult.NotFound:
					break;
			}
		}

		return false;
	}
}