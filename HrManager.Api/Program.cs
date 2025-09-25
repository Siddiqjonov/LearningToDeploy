using HrManager.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var conn = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? throw new InvalidOperationException("DATABASE_URL not configured");

        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));
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

        // Railway injects PORT env var
        app.Urls.Add("http://0.0.0.0:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));
        app.MapControllers();
        app.Run();
    }
}
