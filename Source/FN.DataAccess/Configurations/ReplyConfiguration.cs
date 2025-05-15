using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrKatsuWebAPI.DataAccess.Configurations
{
    public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.ToTable("replies");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.UpdatedAt).IsRequired().HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.ParentId).IsRequired(false);
            builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasOne(x => x.User).WithMany(x => x.Replies).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Topic).WithMany(x => x.Replies).HasForeignKey(x => x.TopicId);
            builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId);
            
            builder.HasIndex(x => x.UpdatedAt).HasDatabaseName("IX_Replies_UpdatedAt");
            builder.HasIndex(x => x.UserId).HasDatabaseName("IX_Replies_UserId");
            builder.HasIndex(x => x.TopicId).HasDatabaseName("IX_Replies_TopicId");
            builder.HasIndex(x => x.IsDeleted).HasDatabaseName("IX_Replies_IsDeleted");

        }
    }
}
