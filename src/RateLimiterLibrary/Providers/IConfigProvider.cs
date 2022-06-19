namespace RateLimiterLibrary.Providers;

public interface IConfigProvider<TConfig>
{
	event Action<TConfig> OnNewConfig;

	TConfig GetCurrentConfig();

	Task<TConfig> GetCurrentConfigAsync();
}