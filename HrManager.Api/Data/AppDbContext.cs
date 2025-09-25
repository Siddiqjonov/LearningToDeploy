using HrManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HrManager.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    public DbSet<User> Users { get; set; }
}
