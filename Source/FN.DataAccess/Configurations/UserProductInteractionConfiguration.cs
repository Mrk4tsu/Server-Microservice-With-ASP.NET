using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class UserProductInteractionConfiguration : IEntityTypeConfiguration<UserProductInteraction>
    {
        public void Configure(EntityTypeBuilder<UserProductInteraction> builder)
        {
            builder.ToTable("user_product_interactions");
            builder.HasKey(x => new { x.UserId, x.ProductId });

            builder.Property(x => x.InteractionDate).HasDefaultValue(DateTime.UtcNow);
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserProductInteractions)
                .HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.ProductDetail)
                .WithMany(x => x.UserProductInteractions)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
