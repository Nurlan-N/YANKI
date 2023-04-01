using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YankiApi.Entities;

namespace YankiApi.Configuration
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.Property(b => b.Key).HasMaxLength(50).IsRequired(true);
            builder.Property(b => b.Value).HasMaxLength(200).IsRequired(true);
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(b => b.CreatedBy).HasDefaultValue("System");
        }
    }
}
