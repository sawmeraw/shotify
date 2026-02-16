using Microsoft.EntityFrameworkCore;
using Shotify.Models;

namespace Shotify.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Brand> Brands { get; set; }
    public DbSet<BrandImageUrl> BrandImageUrls { get; set; }
    public DbSet<BrandImageUrlParam> BrandImageUrlParams { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<BrandImageUrl>(entity =>
        {
            entity.Property(e => e.Order).HasColumnName("Order");
            entity.HasOne(e => e.Brand)
                .WithMany(b => b.BrandImageUrls)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BrandImageUrlParam>(entity =>
        {
            entity.Property(e => e.Order).HasColumnName("Order");
            entity.HasOne(e => e.Brand)
                .WithMany(b => b.BrandImageUrlParams)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.Property(e => e.FetchedAt).HasDefaultValueSql("NOW()");
            entity.HasOne(e => e.Brand)
                .WithMany(b => b.ProductImages)
                .HasForeignKey(e => e.BrandId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
