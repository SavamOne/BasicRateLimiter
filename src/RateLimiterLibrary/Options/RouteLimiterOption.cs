namespace RateLimiterLibrary.Options;

// TODO а почему бы не отнаследовать от defaultLimiterOptions
public class RouteLimiterOption
{
	public TimeSpan WindowSize => TimeSpan.FromMinutes(WindowSizeInMinutes);
	
	public string Path { get; set; }
	
	public int WindowSizeInMinutes { get; set; }
	
	public int RequestsLimit { get; set; }
}