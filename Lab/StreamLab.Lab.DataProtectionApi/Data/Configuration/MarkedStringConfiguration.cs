using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamLab.Lab.DataProtectionApi.Schema;

namespace StreamLab.Lab.DataProtectionApi.Data.Configuration;
public class MarkedStringConfiguration : IEntityTypeConfiguration<MarkedString>
{
    public void Configure(EntityTypeBuilder<MarkedString> builder)
    {
        builder
            .HasOne(x => x.Mark)
            .WithMany(x => x.Strings)
            .HasForeignKey(x => x.MarkId)
            .IsRequired();
    }
}