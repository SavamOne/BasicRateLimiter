using System.ComponentModel.DataAnnotations;

namespace RateLimiterLibrary.Options;

public record IpLimiterOptions : IKeyLimiterOptions<IpLimiterOption>
{
	[Required]
	public IpLimiterOption[] Options { get; set; }
}