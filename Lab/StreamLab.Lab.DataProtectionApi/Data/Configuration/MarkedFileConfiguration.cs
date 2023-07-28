using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Data.Configuration;
public class MarkedFileConfiguration : IEntityTypeConfiguration<MarkedFile>
{
    public void Configure(EntityTypeBuilder<MarkedFile> builder)
    {
        builder
            .HasOne(x => x.Mark)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.MarkId)
            .IsRequired();
    }
}