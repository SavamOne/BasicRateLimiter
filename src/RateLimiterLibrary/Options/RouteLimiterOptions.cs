using System.ComponentModel.DataAnnotations;

namespace RateLimiterLibrary.Options;

public class RouteLimiterOptions
{
	[Required]
	public List<RouteLimiterOption> Options { get; set; }
}