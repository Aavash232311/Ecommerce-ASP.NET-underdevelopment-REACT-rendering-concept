using restaurant_franchise.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;


namespace restaurant_franchise.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Seller> Seller { get; set; }
        public DbSet<Product> Products { get; set; }
        // so that we can delete Product without deleting tags on delete models.cascade
        // protected override void OnModelCreating(ModelBuilder x) {
        //     x.Entity<Product>().HasMany(p => p.related_tags).WithMany().UsingEntity(j => j.ToTable("ProductTags")); // joining tables
        //     base.OnModelCreating(x);
        // }
        public  DbSet<Category> Categories{get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOne(e => e.Parent)
                .WithMany(e => e.Child)
                .HasForeignKey(e => e.CategoryKey)
                .IsRequired(false);
        }
    }
}