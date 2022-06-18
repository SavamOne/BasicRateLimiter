using System.ComponentModel.DataAnnotations;

namespace RateLimiterLibrary.Options;

public class IpLimiterOptions
{
	[Required]
	public List<IpLimiterOption> Options { get; set; }
}