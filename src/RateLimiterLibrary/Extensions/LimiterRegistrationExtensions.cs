using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Providers;
using RateLimiterLibrary.Providers.Implementations;
using RateLimiterLibrary.Services;
using RateLimiterLibrary.Services.Implementations;

namespace RateLimiterLibrary.Extensions;

public static class LimiterRegistrationExtensions
{
	public static IServiceCollection AddDefaultLimiter(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddOptions<DefaultLimiterOptions>()
		   .Bind(configuration.GetSection(nameof(DefaultLimiterOptions)))
		   .Validate(options => options.RequestsLimit <= 0 || options.WindowSizeInMinutes > 0);
		
		serviceCollection.TryAddSingleton<ITimeProvider, StopwatchTicksProvider>();
		serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpContextLimiterService, DefaultHttpContextLimiterService>());
		serviceCollection.TryAddSingleton<IHttpContextLimiterServiceAggregator, HttpContextLimiterServiceAggregator>();

		return serviceCollection;
	}
	
	public static IServiceCollection AddRouteLimiter(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddOptions<RouteLimiterOptions>()
		   .Bind(configuration.GetSection(nameof(RouteLimiterOptions)))
		   .Validate(options => options.Options.All(option => option.RequestsLimit <= 0 || option.WindowSizeInMinutes > 0));
		
		serviceCollection.TryAddSingleton<ITimeProvider, StopwatchTicksProvider>();
		serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpContextLimiterService, RouteHttpContextLimiterService>());
		serviceCollection.TryAddSingleton<IHttpContextLimiterServiceAggregator, HttpContextLimiterServiceAggregator>();
		
		return serviceCollection;
	}
	
	public static IServiceCollection AddIpLimiter(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddOptions<IpLimiterOptions>()
		   .Bind(configuration.GetSection(nameof(IpLimiterOptions)))
		   .Validate(options => options.Options.All(option => IPAddress.TryParse(option.IpAddressStr, out _) && (option.RequestsLimit <= 0 || option.WindowSizeInMinutes > 0)));
		
		serviceCollection.TryAddSingleton<ITimeProvider, StopwatchTicksProvider>();
		serviceCollection.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpContextLimiterService, IpHttpContextLimiterService>());
		serviceCollection.TryAddSingleton<IHttpContextLimiterServiceAggregator, HttpContextLimiterServiceAggregator>();
		
		return serviceCollection;
	}
}