using RateLimiterLibrary.Providers;

namespace RateLimiterLibraryTests.Helpers;

public class TestConfigProvider<TOptions> : IConfigProvider<TOptions>
{
	private TOptions currentValue;

	public TestConfigProvider(TOptions currentValue)
	{
		this.currentValue = currentValue;
	}
	
	public event Action<TOptions>? OnNewConfig;

	public void Set(TOptions value)
	{
		currentValue = value;
		OnNewConfig?.Invoke(value);
	}

	public TOptions GetCurrentConfig()
	{
		return currentValue;
	}
	
	public Task<TOptions> GetCurrentConfigAsync()
	{
		return Task.FromResult(currentValue);
	}
}