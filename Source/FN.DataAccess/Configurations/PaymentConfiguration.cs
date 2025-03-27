using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");
            builder.HasKey(x => x.OrderId);
            builder.Property(x => x.OrderId).ValueGeneratedOnAdd();

            builder.Property(x => x.PaymentDate).HasDefaultValue(DateTime.Now);
            builder.Property(x => x.PaymentStatus).HasDefaultValue(PaymentStatus.PENDING);
            builder.Property(x => x.PaymentFee).HasDefaultValue(0).HasPrecision(18, 2);
            builder.Property(x => x.TransactionId).HasMaxLength(150);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.UserId);
        }
    }
}
