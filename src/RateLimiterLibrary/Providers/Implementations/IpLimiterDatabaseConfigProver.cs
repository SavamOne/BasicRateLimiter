using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using RateLimiterLibrary.Options;

namespace RateLimiterLibrary.Providers.Implementations;

public class IpLimiterDatabaseConfigProver : DatabaseConfigProvider<IpLimiterOptions>
{
	public IpLimiterDatabaseConfigProver(
		Func<IpLimiterOptions, bool> optionsValidator,
		TimeSpan delay, 
		Func<IDbConnection> dbConnectionFactory,
		ILogger<IpLimiterDatabaseConfigProver> logger)
		: base(optionsValidator, delay, dbConnectionFactory, logger) {}

	protected override IpLimiterOptions DefaultValue => new()
	{
		Options = Array.Empty<IpLimiterOption>()
	};

	protected override IpLimiterOptions? GetFromDatabase(IDbConnection dbConnection)
	{
		const string sql = "SELECT * FROM ip_limiter_option";

		var values = dbConnection.Query<IpLimiterOption>(sql).ToArray();

		return !values.Any() ? null : new IpLimiterOptions
		{
			Options = values
		};
	}
}