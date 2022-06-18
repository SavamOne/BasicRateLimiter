using Microsoft.AspNetCore.Http;

namespace RateLimiterLibrary.Services;

public interface IHttpContextLimiterService : IDisposable
{
	CheckResult CanExecuteRequest(HttpContext httpContext);
}