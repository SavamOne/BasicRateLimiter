namespace RateLimiterLibrary.Options;

public interface IKeyLimiterOptions<out TOptions>
{
	TOptions[] Options { get; }
}