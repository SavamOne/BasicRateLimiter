using System.Data.SQLite;
using RateLimiterLibrary.Middlewares;
using RateLimiterLibrary.Registration.Contracts;
using RateLimiterLibrary.Registration.Extensions;

namespace DemoServerApp;

public static class Program
{
    public static void Main(string[] args)
    {
        // Путь до БД. Сама БД находится в <корень репозитория>\db\config.db, но в файле проекта добавлено копирование в bin проекта.
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.db");

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLimiters(builder.Configuration, limiterBuilder =>
        {
            limiterBuilder.UseDefaultLimiter = false;// true;
            limiterBuilder.UseOrderedLimiters = new[]
            {
                LimiterType.RouteLimiter
            };
            limiterBuilder.UseConfigProvider = ConfigProviderType.Database; // ConfigProviderType.OptionMonitor;
            limiterBuilder.DatabaseConnectionFactory = () => new SQLiteConnection($"Data Source={dbPath};Version=3;");
            limiterBuilder.DatabaseCheckPeriod = TimeSpan.FromSeconds(10);
        });

        builder.Services.AddControllers();

        WebApplication app = builder.Build();

        app.UseMiddleware<LimiterMiddleware>();
        app.MapControllers();

        app.Run();
    }
}
