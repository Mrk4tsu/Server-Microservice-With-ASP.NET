using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
    {
        public void Configure(EntityTypeBuilder<ProductItem> builder)
        {
            builder.ToTable("product_items");
            builder.HasKey(x => x.Id);
            builder.Property(x =>x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(500);
            builder.HasOne(x => x.ProductDetail)
                .WithMany(x => x.ProductItems)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
