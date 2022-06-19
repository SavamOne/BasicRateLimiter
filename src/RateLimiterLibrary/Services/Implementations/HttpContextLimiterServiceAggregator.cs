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
		// TODO Тааааак, а вот если мы все три регистрируем... то в каком порядке они будут проверяться? а если одна проверка будет валидной, а вторая нет?
		// чисто в теории могут быть не верные результаты
		// Или как это ты себе представлял?
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