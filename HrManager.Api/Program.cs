using HrManager.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (string.IsNullOrWhiteSpace(connUrl))
            throw new InvalidOperationException("DATABASE_URL not configured");

        var uri = new Uri(connUrl);
        var userInfo = uri.UserInfo.Split(':');
        var connStr =
            $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};" +
            $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connStr));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
