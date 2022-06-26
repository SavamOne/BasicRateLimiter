using System.Data;
using RateLimiterLibrary.Registration.Contracts;

namespace RateLimiterLibrary.Registration.Builders;

public class LimiterBuilder
{
	public IReadOnlyCollection<LimiterType> UseOrderedLimiters { get; set; } = new List<LimiterType>();

	public bool UseDefaultLimiter { get; set; } = true;

	public ConfigProviderType UseConfigProvider { get; set; } = ConfigProviderType.OptionMonitor;
	
	public TimeSpan DatabaseCheckPeriod { get; set; } = TimeSpan.FromSeconds(20);
	
	public Func<IDbConnection>? DatabaseConnectionFactory { get; set; }
}