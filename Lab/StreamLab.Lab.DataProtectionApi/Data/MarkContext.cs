using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Data;
public class MarkContext : DbContext
{
    public MarkContext(DbContextOptions<MarkContext> options) : base(options) { }

    public DbSet<Mark> Marks => Set<Mark>();
    public DbSet<MarkedFile> Files => Set<MarkedFile>();
    public DbSet<MarkedObject> Objects => Set<MarkedObject>();
    public DbSet<MarkedString> Strings => Set<MarkedString>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly()
        );
    }
}