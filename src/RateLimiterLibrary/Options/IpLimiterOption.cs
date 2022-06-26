using System.Net;

namespace RateLimiterLibrary.Options;

public record IpLimiterOption : DefaultLimiterOptions
{
	public IPAddress IpAddress => IPAddress.TryParse(IpAddressStr, out var address) ? address : IPAddress.Any;
	
	public string IpAddressStr { get; set; }
}