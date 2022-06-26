using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using RateLimiterLibrary.Options;

namespace RateLimiterLibrary.Providers.Implementations;

public class RouteLimiterDatabaseConfigProvider : DatabaseConfigProvider<RouteLimiterOptions>
{
	public RouteLimiterDatabaseConfigProvider(Func<RouteLimiterOptions, bool> optionsValidator,
		TimeSpan delay,
		Func<IDbConnection> dbConnectionFactory,
		ILogger<RouteLimiterDatabaseConfigProvider> logger)
		: base(optionsValidator, delay, dbConnectionFactory, logger) {}

	protected override RouteLimiterOptions DefaultValue => new()
	{
		Options = Array.Empty<RouteLimiterOption>()
	};
	
	protected override RouteLimiterOptions? GetFromDatabase(IDbConnection dbConnection)
	{
		const string sql = "SELECT * FROM route_limiter_option";

		var values = dbConnection.Query<RouteLimiterOption>(sql).ToArray();

		return !values.Any() ? null : new RouteLimiterOptions
		{
			Options = values
		};
	}
}