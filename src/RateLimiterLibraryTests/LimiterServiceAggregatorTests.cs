using System.Net;
using Microsoft.AspNetCore.Http;
using Moq;
using RateLimiterLibrary.Options;
using RateLimiterLibrary.Services;
using RateLimiterLibrary.Services.Implementations;
using RateLimiterLibraryTests.Helpers;
using Xunit;

namespace RateLimiterLibraryTests;

public class LimiterServiceAggregatorTests
{
	[Fact]
	public void CanExecuteRequest_WhenKeyedLimiterIsBeforeStandardLimiterAndKeyPassLimit_MustNotAffectStandardLimiter()
	{
		HttpContext firstHttpContext = CreateHttpContextMockWithRequestPath("/test1");
		HttpContext secondHttpContext = CreateHttpContextMockWithRequestPath("/test2");

		var routeLimiterConfigProvider = new TestConfigProvider<RouteLimiterOptions>(CreateRouteLimiterOptions("/test1", 1, 1));
		var standardLimiterConfigProvider = new TestConfigProvider<DefaultLimiterOptions>(CreateDefaultLimiterOptions(1, 1));

		TestTimeProvider timeProvider = new();
		using RouteHttpContextLimiterService routeLimiter = new(timeProvider, routeLimiterConfigProvider);
		DefaultHttpContextLimiterService defaultLimiter = new(timeProvider, standardLimiterConfigProvider);
		HttpContextLimiterServiceAggregator aggregator = new(new IHttpContextLimiterService[]{ routeLimiter, defaultLimiter });

		bool result = aggregator.CanExecuteRequest(firstHttpContext);
		Assert.True(result);
		
		result = aggregator.CanExecuteRequest(firstHttpContext);
		Assert.False(result);
		
		result = aggregator.CanExecuteRequest(secondHttpContext);
		Assert.True(result);
	}

	[Fact]
	public void CanExecuteRequest_WhenLimiterSetAndOptionsUpdated_MustUseNewOptions()
	{
		HttpContext httpContext = CreateHttpContextMockWithRemoteIpAddress("1.1.1.1");
		
		var ipLimiterConfigProvider = new TestConfigProvider<IpLimiterOptions>(CreateIpLimiterOptions("1.1.1.1", 2, 1));
		
		TestTimeProvider timeProvider = new();
		using IpHttpContextLimiterService ipLimiter = new(timeProvider, ipLimiterConfigProvider);
		HttpContextLimiterServiceAggregator aggregator = new(new IHttpContextLimiterService[]{ ipLimiter });
		
		bool result = aggregator.CanExecuteRequest(httpContext);
		Assert.True(result);
		
		result = aggregator.CanExecuteRequest(httpContext);
		Assert.True(result);
		
		result = aggregator.CanExecuteRequest(httpContext);
		Assert.False(result);
		
		ipLimiterConfigProvider.Set(CreateIpLimiterOptions("1.1.1.1", 3, 1));
		
		result = aggregator.CanExecuteRequest(httpContext);
		Assert.True(result);
		
		result = aggregator.CanExecuteRequest(httpContext);
		Assert.False(result);
	}

	private static HttpContext CreateHttpContextMockWithRequestPath(string path)
	{
		var mock = new Mock<HttpContext>
		{
			DefaultValue = DefaultValue.Mock
		};
		mock.Setup(httpContext => httpContext.Request.Path).Returns(() => path);

		return mock.Object;
	}
	
	private static HttpContext CreateHttpContextMockWithRemoteIpAddress(string address)
	{
		var mock = new Mock<HttpContext>
		{
			DefaultValue = DefaultValue.Mock
		};
		mock.Setup(httpContext => httpContext.Connection.RemoteIpAddress).Returns(() => IPAddress.Parse(address));

		return mock.Object;
	}

	private DefaultLimiterOptions CreateDefaultLimiterOptions(int requestLimit, int windowLimitInMinutes)
	{
		return new DefaultLimiterOptions
		{
			RequestsLimit = requestLimit,
			WindowSizeInMinutes = windowLimitInMinutes
		};
	}
	
	private static RouteLimiterOptions CreateRouteLimiterOptions(string path, int requestLimit, int windowLimitInMinutes)
	{
		return new RouteLimiterOptions
		{
			Options = new List<RouteLimiterOption>
			{
				new ()
				{
					Path = path, 
					RequestsLimit = requestLimit, 
					WindowSizeInMinutes = windowLimitInMinutes
				}
			}
		};
	}
	
	private static IpLimiterOptions CreateIpLimiterOptions(string address, int requestLimit, int windowLimitInMinutes)
	{
		return new IpLimiterOptions
		{
			Options = new List<IpLimiterOption>
			{
				new()
				{
					IpAddressStr = address,
					RequestsLimit = requestLimit,
					WindowSizeInMinutes = windowLimitInMinutes
				}
			}
		};
	}
}