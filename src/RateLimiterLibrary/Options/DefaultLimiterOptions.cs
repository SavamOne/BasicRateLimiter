namespace RateLimiterLibrary.Options;

public record DefaultLimiterOptions
{
	public TimeSpan WindowSize => TimeSpan.FromMinutes(WindowSizeInMinutes);
	
	public int WindowSizeInMinutes { get; set; }
	
	public int RequestsLimit { get; set; } 
}