using System.ComponentModel.DataAnnotations;

namespace RateLimiterLibrary.Options;

public record RouteLimiterOptions : IKeyLimiterOptions<RouteLimiterOption>
{
    [Required]
    public RouteLimiterOption[] Options { get; set; }
}