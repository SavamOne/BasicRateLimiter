using System.Data;
using Microsoft.Extensions.Logging;
using RateLimiterLibrary.Options;

namespace RateLimiterLibrary.Providers.Implementations;

public abstract class DatabaseConfigProvider<TOptions> : IConfigProvider<TOptions>, IDisposable
	where TOptions : class
{
	private readonly CancellationTokenSource tcs;
	private readonly TimeSpan delay;
	private readonly Func<TOptions, bool> optionsValidator;
	private readonly Func<IDbConnection> dbConnectionFactory;
	private readonly ILogger logger;

	private TOptions currentValue;

	protected DatabaseConfigProvider(
		Func<TOptions, bool> optionsValidator,
		TimeSpan delay, 
		Func<IDbConnection> dbConnectionFactory, 
		ILogger logger)
	{
		this.logger = logger;
		this.optionsValidator = optionsValidator;
		this.dbConnectionFactory = dbConnectionFactory;
		this.delay = delay;
		
		tcs = new CancellationTokenSource();
		currentValue = GetConfig() ?? DefaultValue;
		_ = ConfigWorker(tcs.Token);
	}

	public event Action<TOptions>? OnNewConfig;

	public TOptions GetCurrentConfig()
	{
		return currentValue;
	}
	
	public Task<TOptions> GetCurrentConfigAsync()
	{
		return Task.FromResult(currentValue);
	}
	
	protected abstract TOptions DefaultValue { get; }

	protected abstract TOptions? GetFromDatabase(IDbConnection dbConnection);

	private TOptions? GetConfig()
	{
		using IDbConnection connection = dbConnectionFactory();

		TOptions? config = GetFromDatabase(connection);
		
		if (config is not null && optionsValidator(config))
		{
			return config;
		}
		
		logger.LogWarning("Config does not exists or incorrect");
		return null;
	}

	private async Task ConfigWorker(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			await Task.Delay(delay, cancellationToken);
			
			try
			{
				TOptions? config = GetConfig();
				if (config is null || Equals(config, currentValue))
				{
					continue;
				}

				Interlocked.Exchange(ref currentValue, config);
				OnNewConfig?.Invoke(currentValue);
				logger.LogInformation("New config applied");
			}
			catch(Exception e)
			{
				logger.LogError(e, "Error while getting config. Using last successful value");
			}
		}
	}

	private static bool Equals(TOptions options, TOptions currentOptions)
	{
		if (options is IKeyLimiterOptions<object> optionsEnumerable)
		{
			return optionsEnumerable.Options.SequenceEqual(((IKeyLimiterOptions<object>)currentOptions).Options);
		}

		return options.Equals(currentOptions);
	}
	
	public void Dispose()
	{
		tcs.Cancel();
		tcs.Dispose();
	}
}