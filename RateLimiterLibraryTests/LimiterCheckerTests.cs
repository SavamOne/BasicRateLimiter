using RateLimiterLibrary.Checkers;
using RateLimiterLibraryTests.Helpers;
using Xunit;

namespace RateLimiterLibraryTests;

public class LimiterCheckerTests
{
	[Fact]
	public void CanExecuteRequest_WhenWindowLimitIsOver_MustReturnFalseUntilNextWindow()
	{
		TestTimeProvider timeProvider = new ();
		LimiterChecker limiterChecker = new(timeProvider, 2, TimeSpan.FromMinutes(10));
		
		bool result = limiterChecker.CanExecuteRequest();
		Assert.True(result);
		
		result = limiterChecker.CanExecuteRequest();
		Assert.True(result);
		
		result = limiterChecker.CanExecuteRequest();
		Assert.False(result);
			
		timeProvider.Add(TimeSpan.FromMinutes(10));
			
		result = limiterChecker.CanExecuteRequest();
		Assert.True(result);
	}

	[Fact]
	public void CanExecuteRequest_WhenWindowLimitIsOverAndOptionsUpdates_MustAffectChangesAndNotDroppingCurrentWindowSettings()
	{
		TimeSpan windowSize = TimeSpan.FromMinutes(10);
		TestTimeProvider timeProvider = new ();
		LimiterChecker limiterChecker = new(timeProvider, 3, windowSize);
		
		limiterChecker.CanExecuteRequest();

		bool result = limiterChecker.CanExecuteRequest();
		Assert.True(result);

		limiterChecker.UpdateLimits(2, windowSize);

		result = limiterChecker.CanExecuteRequest();
		Assert.False(result);
	}
	
	[Fact]
	public void CanExecuteRequest_WhenWindowLimitIsLessOrEqualsZero_MustAlwaysReturnTrue()
	{
		TimeSpan windowSize = TimeSpan.FromMinutes(10);
		TestTimeProvider timeProvider = new ();
		LimiterChecker limiterChecker = new(timeProvider, 0, windowSize);

		bool result = true;
		for (int i = 0; i < 1000; i++)
		{
			result &= limiterChecker.CanExecuteRequest();
		}
		
		Assert.True(result);
	}
}