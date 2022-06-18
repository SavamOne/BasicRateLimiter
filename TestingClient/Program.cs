namespace TestingClient;

public static class Program
{
	public static async Task Main()
	{
		using CancellationTokenSource cts = new();
		
		Task firstClientTask = Task.Run(async () =>
		{
			using HttpClient firstClient = new();
			while (!cts.IsCancellationRequested)
			{
				await firstClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:5073/weatherforecast"), cts.Token);
			}
		});
		
		Task secondClientTask = Task.Run(async () =>
		{
			using HttpClient secondClient = new();
			while (!cts.IsCancellationRequested)
			{
				await secondClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://localhost:5073/weatherforecast"), cts.Token);
			}
		});

		Console.ReadLine();

		cts.Cancel();

		await firstClientTask;
		await secondClientTask;

	}
}