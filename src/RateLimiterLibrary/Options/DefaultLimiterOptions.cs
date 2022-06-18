namespace RateLimiterLibrary.Options;

public class DefaultLimiterOptions
{
	public TimeSpan WindowSize => TimeSpan.FromMinutes(WindowSizeInMinutes);
	
	public int WindowSizeInMinutes { get; set; }
	
	public int RequestsLimit { get; set; } 
}