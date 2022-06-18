using Microsoft.AspNetCore.Http;

namespace RateLimiterLibrary.Services;

public interface IHttpContextLimiterServiceAggregator
{
	bool CanExecuteRequest(HttpContext httpContext);
}