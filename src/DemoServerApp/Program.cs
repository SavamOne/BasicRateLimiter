using RateLimiterLibrary.Extensions;
using RateLimiterLibrary.Middlewares;

namespace DemoServerApp;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services
           .AddRouteLimiter(builder.Configuration)
           // .AddIpLimiter(builder.Configuration)
           .AddDefaultLimiter(builder.Configuration);
		builder.Services.AddControllers();

        WebApplication app = builder.Build();

        app.UseMiddleware<LimiterMiddleware>();
        app.MapControllers();

        app.Run();
    }
}
