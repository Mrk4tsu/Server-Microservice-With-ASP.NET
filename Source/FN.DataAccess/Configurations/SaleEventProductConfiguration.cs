using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class SaleEventProductConfiguration : IEntityTypeConfiguration<SaleEventProduct>
    {
        public void Configure(EntityTypeBuilder<SaleEventProduct> builder)
        {
            builder.ToTable("sale_event_products");
            builder.HasKey(sep => sep.Id);
            builder.Property(sep => sep.Id).ValueGeneratedOnAdd();
            builder.Property(sep => sep.DiscountedPrice).HasPrecision(18, 2).HasDefaultValue(0);
            builder.Property(sep => sep.MaxPurchases).HasDefaultValue(0);
            builder.Property(sep => sep.CurrentPurchases).HasDefaultValue(0);
            builder.Property(sep => sep.IsActive).HasDefaultValue(true);

            builder.HasOne(sep => sep.SaleEvent)
                .WithMany(se => se.Products)
                .HasForeignKey(sep => sep.SaleEventId);
            builder.HasOne(sep => sep.ProductDetail).WithMany(pd => pd.SaleEventProducts)
                .HasForeignKey(sep => sep.ProductDetailId);
        }
    }
}
