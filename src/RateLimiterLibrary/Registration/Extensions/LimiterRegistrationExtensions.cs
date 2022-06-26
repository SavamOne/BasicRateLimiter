using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Providers;
using RateLimiterLibrary.Providers.Implementations;
using RateLimiterLibrary.Registration.Builders;
using RateLimiterLibrary.Registration.Contracts;
using RateLimiterLibrary.Services;
using RateLimiterLibrary.Services.Implementations;

namespace RateLimiterLibrary.Registration.Extensions;

public static class LimiterRegistrationExtensions
{
	public static IServiceCollection AddLimiters(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		return AddLimiters(serviceCollection, configuration, _ => {});
	}
	
	public static IServiceCollection AddLimiters(this IServiceCollection serviceCollection, IConfiguration configuration, Action<LimiterBuilder> configure)
	{
		serviceCollection.TryAddSingleton<ITimeProvider, StopwatchTicksProvider>();

		LimiterBuilder builder = new();
		configure(builder);

		if (builder.UseConfigProvider is ConfigProviderType.Database)
		{
			DefaultTypeMap.MatchNamesWithUnderscores = true;
			serviceCollection.AddDatabasedLimiters(builder);
		}
		else
		{
			serviceCollection.AddOptionsMonitorLimiters(configuration, builder);
		}
		
		serviceCollection.TryAddSingleton<IHttpContextLimiterServiceAggregator, HttpContextLimiterServiceAggregator>();
		return serviceCollection;
	}
	
	private static void AddOptionsMonitorLimiters(this IServiceCollection serviceCollection, IConfiguration configuration, LimiterBuilder builder)
	{
		foreach (LimiterType type in builder.UseOrderedLimiters)
		{
			serviceCollection.AddLimiterOptionMonitorConfig(configuration, type);
			serviceCollection.AddLimiter(type);
		}

		if (builder.UseDefaultLimiter)
		{
			serviceCollection.AddLimiterOptionMonitorConfig(configuration);
			serviceCollection.AddLimiter();
		}
	}
	
	private static void AddDatabasedLimiters(this IServiceCollection serviceCollection, LimiterBuilder builder)
	{
		if (builder.DatabaseCheckPeriod <= TimeSpan.Zero)
		{
			throw new ValidationException(nameof(builder.DatabaseCheckPeriod));
		}
		if (builder.DatabaseConnectionFactory is null)
		{
			throw new ValidationException(nameof(builder.DatabaseConnectionFactory));
		}

		foreach (LimiterType type in builder.UseOrderedLimiters)
		{
			serviceCollection.AddKeyLimiterDatabaseConfig(builder.DatabaseCheckPeriod, builder.DatabaseConnectionFactory, type);
			serviceCollection.AddLimiter(type);
		}

		if (builder.UseDefaultLimiter)
		{
			serviceCollection.AddKeyLimiterDatabaseConfig(builder.DatabaseCheckPeriod, builder.DatabaseConnectionFactory);
			serviceCollection.AddLimiter();
		}
	}

	private static void AddLimiterOptionMonitorConfig(this IServiceCollection serviceCollection, IConfiguration configuration, LimiterType? limiterType = null)
	{
		switch (limiterType)
		{
			case LimiterType.RouteLimiter:
				serviceCollection.AddOptions<RouteLimiterOptions>()
				   .Bind(configuration.GetSection(nameof(RouteLimiterOptions)))
				   .Validate(RouteLimiterOptionsValidator);
				serviceCollection.AddSingleton<IConfigProvider<RouteLimiterOptions>, OptionsMonitorConfigProvider<RouteLimiterOptions>>();
				break;
			case LimiterType.IpLimiter:
				serviceCollection.AddOptions<IpLimiterOptions>()
				   .Bind(configuration.GetSection(nameof(IpLimiterOptions)))
				   .Validate(IpLimiterOptionsValidator);
				serviceCollection.AddSingleton<IConfigProvider<IpLimiterOptions>, OptionsMonitorConfigProvider<IpLimiterOptions>>();
				break;
			case null:
				serviceCollection.AddOptions<DefaultLimiterOptions>()
				   .Bind(configuration.GetSection(nameof(DefaultLimiterOptions)))
				   .Validate(DefaultOptionsValidator);
				serviceCollection.AddSingleton<IConfigProvider<DefaultLimiterOptions>, OptionsMonitorConfigProvider<DefaultLimiterOptions>>();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(limiterType));
		}
	}
	
	private static void AddKeyLimiterDatabaseConfig(this IServiceCollection serviceCollection, TimeSpan checkPeriod, Func<IDbConnection> dbConnectionProvider, LimiterType? limiterType = null)
	{
		switch (limiterType)
		{
			case LimiterType.RouteLimiter:
				serviceCollection.AddSingleton<IConfigProvider<RouteLimiterOptions>>(provider =>
				{
					var logger = provider.GetRequiredService<ILogger<RouteLimiterDatabaseConfigProvider>>();
					return new RouteLimiterDatabaseConfigProvider(RouteLimiterOptionsValidator, checkPeriod, dbConnectionProvider, logger);
				});
				break;
			case LimiterType.IpLimiter:
				serviceCollection.AddSingleton<IConfigProvider<IpLimiterOptions>>(provider =>
				{
					var logger = provider.GetRequiredService<ILogger<IpLimiterDatabaseConfigProver>>();
					return new IpLimiterDatabaseConfigProver(IpLimiterOptionsValidator, checkPeriod, dbConnectionProvider, logger);
				});
				break;
			case null:
				serviceCollection.AddSingleton<IConfigProvider<DefaultLimiterOptions>>(provider =>
				{
					var logger = provider.GetRequiredService<ILogger<DefaultLimiterDatabaseConfigProvider>>();
					return new DefaultLimiterDatabaseConfigProvider(DefaultOptionsValidator, checkPeriod, dbConnectionProvider, logger);
				});
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(limiterType));
		}
	}
	
	private static void AddLimiter(this IServiceCollection serviceCollection, LimiterType? limiterType = null)
	{
		ServiceDescriptor descriptor = limiterType switch
		{
			LimiterType.IpLimiter => ServiceDescriptor.Singleton<IHttpContextLimiterService, IpHttpContextLimiterService>(),
			LimiterType.RouteLimiter => ServiceDescriptor.Singleton<IHttpContextLimiterService, RouteHttpContextLimiterService>(),
			null => ServiceDescriptor.Singleton<IHttpContextLimiterService, DefaultHttpContextLimiterService>(),
			_ => throw new ArgumentOutOfRangeException(nameof(limiterType))
		};
		
		serviceCollection.TryAddEnumerable(descriptor);
	}

	private static bool DefaultOptionsValidator(DefaultLimiterOptions options)
	{
		return options.RequestsLimit <= 0 || options.WindowSizeInMinutes > 0;
	}
	
	private static bool IpLimiterOptionsValidator(IpLimiterOptions options)
	{
		return options.Options.All(option => IPAddress.TryParse(option.IpAddressStr, out _) && DefaultOptionsValidator(option));
	}

	private static bool RouteLimiterOptionsValidator(RouteLimiterOptions options)
	{
		return options.Options.All(DefaultOptionsValidator);
	}
}