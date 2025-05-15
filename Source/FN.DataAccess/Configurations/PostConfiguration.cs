using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrKatsuWebAPI.DataAccess.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("topics");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.UpdatedAt).IsRequired().HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasOne(x => x.User).WithMany(x => x.Topics).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Replies).WithOne(x => x.Topic).HasForeignKey(x => x.TopicId);
            builder.Property(x => x.IsLocked).IsRequired().HasDefaultValue(false);

            builder.HasIndex(x => x.UpdatedAt).HasDatabaseName("IX_Topics_UpdatedAt");
            builder.HasIndex(x => x.UserId).HasDatabaseName("IX_Topics_UserId");
            builder.HasIndex(x => x.IsDeleted).HasDatabaseName("IX_Topics_IsDeleted");
            builder.HasIndex(x => x.IsLocked).HasDatabaseName("IX_Topics_IsLocked");
        }
    }
}
