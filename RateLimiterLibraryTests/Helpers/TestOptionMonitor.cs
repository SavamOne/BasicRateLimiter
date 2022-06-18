using Microsoft.Extensions.Options;
using Moq;

namespace RateLimiterLibraryTests.Helpers;

public class TestOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
{
	private Action<TOptions, string>? listener;

	public TestOptionsMonitor(TOptions currentValue)
	{
		CurrentValue = currentValue;
	}

	public TOptions CurrentValue { get; private set; }

	public TOptions Get(string name)
	{
		return CurrentValue;
	}

	public void Set(TOptions value)
	{
		CurrentValue = value;
		listener?.Invoke(value, string.Empty);
	}

	public IDisposable OnChange(Action<TOptions, string> listener)
	{
		this.listener = listener;
		return Mock.Of<IDisposable>();
	}
}