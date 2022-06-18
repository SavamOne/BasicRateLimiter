namespace RateLimiterLibrary.Options;

public class RouteLimiterOption
{
	public TimeSpan WindowSize => TimeSpan.FromMinutes(WindowSizeInMinutes);
	
	public string Path { get; set; }
	
	public int WindowSizeInMinutes { get; set; }
	
	public int RequestsLimit { get; set; }
}