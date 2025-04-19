using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FN.DataAccess.Configurations
{
    public class SaleEventConfiguration : IEntityTypeConfiguration<SaleEvent>
    {
        public void Configure(EntityTypeBuilder<SaleEvent> builder)
        {
            builder.ToTable("sale_events");
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.StartDate).HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.EndDate).HasDefaultValue(DateTime.UtcNow.AddDays(7));
            builder.Property(x => x.IsActive).HasDefaultValue(true);
        }
    }
}
