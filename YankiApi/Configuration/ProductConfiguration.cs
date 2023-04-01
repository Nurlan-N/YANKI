using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YankiApi.Entities;

namespace YankiApi.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(b => b.Title).HasMaxLength(200).IsRequired(true);
            builder.Property(b => b.Image).HasMaxLength(250).IsRequired(true);
            builder.Property(b => b.Description).HasMaxLength(1000).IsRequired(true);
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(b => b.CreatedBy).HasDefaultValue("System");
        }
    }
}
