using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class UserBlogInteractionConfiguration : IEntityTypeConfiguration<UserBlogInteraction>
    {
        public void Configure(EntityTypeBuilder<UserBlogInteraction> builder)
        {
            builder.ToTable("user_blog_interactions");
            builder.HasKey(x => new { x.UserId, x.BlogId });
            builder.Property(x => x.InteractionDate).HasDefaultValue(DateTime.Now);

            builder.HasOne(x => x.User).WithMany(x => x.Interactions).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Blog).WithMany(x => x.Interactions).HasForeignKey(x => x.BlogId);
        }
    }
}
