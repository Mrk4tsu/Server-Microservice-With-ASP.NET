using FN.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FN.DataAccess
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("user_roles").HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("user_logins").HasKey(x => x.UserId);

            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("user_tokens").HasKey(x => x.UserId);

            modelBuilder.Seed();
        }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<ProductOwner> ProductOwners { get; set; }
        public DbSet<BlogImage> BlogsImages { get; set; }
        public DbSet<UserBlogInteraction> UserBlogInteractions { get; set; }
        public DbSet<UserOrder> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserProductInteraction> UserProductInteractions { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
    }
}
