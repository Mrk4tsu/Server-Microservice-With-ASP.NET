using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class BlogImageConfiguration : IEntityTypeConfiguration<BlogImage>
    {
        public void Configure(EntityTypeBuilder<BlogImage> builder)
        {
            builder.ToTable("blog_images");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.PublicId).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Caption).HasMaxLength(150);
            builder.Property(x => x.ImageUrl).HasMaxLength(250).IsRequired();
            builder.HasOne(x => x.Blog).WithMany(x => x.BlogImages).HasForeignKey(x => x.BlogId);
        }
    }
}
