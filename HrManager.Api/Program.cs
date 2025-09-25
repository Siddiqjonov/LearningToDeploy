using HrManager.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var conn = $"Host={Environment.GetEnvironmentVariable("PGHOST")};" +
                   $"Port={Environment.GetEnvironmentVariable("PGPORT") ?? "5432"};" +
                   $"Username={Environment.GetEnvironmentVariable("PGUSER")};" +
                   $"Password={Environment.GetEnvironmentVariable("PGPASSWORD")};" +
                   $"Database={Environment.GetEnvironmentVariable("PGDATABASE")};Pooling=true;";

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

        // Allow Railway to set the port
        var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
        app.Urls.Add($"http://*:{port}");

        app.MapControllers();
        app.Run();
    }
}
