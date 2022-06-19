using System.Net;

namespace RateLimiterLibrary.Options;

// TODO а почему бы не отнаследовать от defaultLimiterOptions
public class IpLimiterOption
{
	public TimeSpan WindowSize => TimeSpan.FromMinutes(WindowSizeInMinutes);
	
	public IPAddress IpAddress => IPAddress.TryParse(IpAddressStr, out var address) ? address : IPAddress.Any;
	
	public string IpAddressStr { get; set; }
	
	public int WindowSizeInMinutes { get; set; }
	
	public int RequestsLimit { get; set; } 
}