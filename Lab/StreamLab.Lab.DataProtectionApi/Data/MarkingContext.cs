using Microsoft.EntityFrameworkCore;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Data;
public class MarkingContext : DbContext
{
    public MarkingContext(DbContextOptions<MarkingContext> options) : base(options) { }

    public DbSet<Marking> Markings => Set<Marking>();
}