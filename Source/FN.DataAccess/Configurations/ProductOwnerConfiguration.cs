using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class ProductOwnerConfiguration : IEntityTypeConfiguration<ProductOwner>
    {
        public void Configure(EntityTypeBuilder<ProductOwner> builder)
        {
            builder.ToTable("product_owners");
            builder.HasKey(x => new 
            { 
                x.UserId,
                x.ProductId 
            });
            builder.HasOne(x => x.User)
                .WithMany(x => x.ProductOwners)
                .HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductOwners)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
