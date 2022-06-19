using Microsoft.AspNetCore.Http;
using RateLimiterLibrary.Contracts;

namespace RateLimiterLibrary.Services;

public interface IHttpContextLimiterService
{
	CheckResult CanExecuteRequest(HttpContext httpContext);
}