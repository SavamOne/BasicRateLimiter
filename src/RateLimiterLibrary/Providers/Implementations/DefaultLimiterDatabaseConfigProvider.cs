using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using RateLimiterLibrary.Options;

namespace RateLimiterLibrary.Providers.Implementations;

public class DefaultLimiterDatabaseConfigProvider : DatabaseConfigProvider<DefaultLimiterOptions>
{
	public DefaultLimiterDatabaseConfigProvider(
		Func<DefaultLimiterOptions, bool> optionsValidator,
		TimeSpan delay,
		Func<IDbConnection> dbConnectionFactory,
		ILogger<DefaultLimiterDatabaseConfigProvider> logger)
		: base(optionsValidator, delay, dbConnectionFactory, logger) {}

	protected override DefaultLimiterOptions DefaultValue => new();

	protected override DefaultLimiterOptions GetFromDatabase(IDbConnection dbConnection)
	{
		const string sql = "SELECT * FROM default_option LIMIT 1";
		
		return dbConnection.QueryFirstOrDefault<DefaultLimiterOptions>(sql);
	}
}