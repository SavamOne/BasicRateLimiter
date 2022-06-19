using Microsoft.Extensions.Options;

namespace RateLimiterLibrary.Providers.Implementations;

public class OptionsMonitorConfigProvider<TConfig> : IConfigProvider<TConfig>, IDisposable
{
	private readonly IOptionsMonitor<TConfig> optionsMonitor;
	private readonly IDisposable changeTracker;
	
	public OptionsMonitorConfigProvider(IOptionsMonitor<TConfig> optionsMonitor)
	{
		this.optionsMonitor = optionsMonitor;
		changeTracker = optionsMonitor.OnChange(config => OnNewConfig?.Invoke(config));
	}
	
	public event Action<TConfig>? OnNewConfig;

	public TConfig GetCurrentConfig()
	{
		return optionsMonitor.CurrentValue;
	}
	
	public Task<TConfig> GetCurrentConfigAsync()
	{
		return Task.FromResult(GetCurrentConfig());
	}

	public void Dispose()
	{
		changeTracker.Dispose();
	}
}