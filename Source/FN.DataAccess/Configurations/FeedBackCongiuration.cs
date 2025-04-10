using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class FeedBackCongiuration : IEntityTypeConfiguration<FeedBack>
    {
        public void Configure(EntityTypeBuilder<FeedBack> builder)
        {
            builder.ToTable("feedbacks");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(x => x.Rate).HasDefaultValue(5);
            builder.Property(x => x.TimeCreated)
                .HasDefaultValue(DateTime.Now);
            builder.Property(x => x.Status).HasDefaultValue(true);
            builder.HasOne(x => x.User)
                .WithMany(x => x.FeedBacks)
                .HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.ProductDetail)
                .WithMany(x => x.FeedBacks)
                .HasForeignKey(x => x.ProductId);
        }
    }
}
