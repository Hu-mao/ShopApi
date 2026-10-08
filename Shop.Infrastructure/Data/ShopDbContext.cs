using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shop.Domain.Entities;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Data;

public class ShopDbContext:DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }\n    public DbSet<Provider> Providers { get; set; }\n    public DbSet<UserProvider> UserProviders { get; set; }
    // Автоматично встановлює CreatedAt і UpdatedAt перед збереженням
    public override int SaveChanges()
    {
        SetTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity => {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserProvider>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.UserId, x.ProviderId })
                .IsUnique();

            entity.HasIndex(x => new { x.ProviderId, x.NumberProvider })
                .IsUnique();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Provider)
                .WithMany(x => x.UserProviders)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Category ---
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();

            entity.HasOne(c => c.Parent)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(c => c.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Product ---
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price)
                  .HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict); //забороняємо видалення
        });

        // --- ProductImage ---
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasOne(i => i.Product)
                  .WithMany(p => p.Images)
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        // --- Order ---
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(o => o.Status)
                .HasMaxLength(50);
        });

        // --- OrderDetail ---
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasOne(d => d.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(d => d.Price)
                .HasColumnType("decimal(18,2)");
        });
        modelBuilder.Entity<DeliveryAddress>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Street)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.House)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Apartment)
                .HasMaxLength(20);

            entity.Property(x => x.PostalCode)
                .HasMaxLength(20);

            entity.HasOne(x => x.User)
                .WithMany(x => x.DeliveryAddresses)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
