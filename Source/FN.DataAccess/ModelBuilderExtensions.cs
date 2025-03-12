using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FN.DataAccess
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Admintrator",
                },
                new AppRole
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "Admintrator",
                }
            );
        }
    }
}
