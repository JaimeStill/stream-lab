using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Data.Configuration;
public class MarkedObjectConfiguration : IEntityTypeConfiguration<MarkedObject>
{
    public void Configure(EntityTypeBuilder<MarkedObject> builder)
    {
        builder
            .HasOne(x => x.Mark)
            .WithMany(x => x.Objects)
            .HasForeignKey(x => x.MarkId)
            .IsRequired();
    }
}