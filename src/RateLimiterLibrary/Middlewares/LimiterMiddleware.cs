using Microsoft.AspNetCore.Http;
using RateLimiterLibrary.Services;

namespace RateLimiterLibrary.Middlewares;

public class LimiterMiddleware
{
	private readonly IHttpContextLimiterServiceAggregator serviceAggregator;
	private readonly RequestDelegate next;

	public LimiterMiddleware(IHttpContextLimiterServiceAggregator serviceAggregator, RequestDelegate next)
	{
		this.serviceAggregator = serviceAggregator;
		this.next = next;
	}
	
	public async Task InvokeAsync(HttpContext context)
	{
		if (serviceAggregator.CanExecuteRequest(context))
		{
			await next(context);
			return;
		}

		context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
	}
	
}