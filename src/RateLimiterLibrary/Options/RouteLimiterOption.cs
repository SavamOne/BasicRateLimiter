namespace RateLimiterLibrary.Options;

public record RouteLimiterOption : DefaultLimiterOptions
{
	public string Path { get; set; }
}