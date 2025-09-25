using HrManager.Api.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HrManager.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (string.IsNullOrEmpty(databaseUrl))
            throw new InvalidOperationException("DATABASE_URL not configured");

        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        var connStr = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = uri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        }.ToString();

        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connStr));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!app.Environment.IsProduction())
            app.UseHttpsRedirection();

        app.MapControllers();
        app.Run();
    }
}
