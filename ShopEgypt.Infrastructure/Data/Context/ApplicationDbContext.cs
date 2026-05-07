using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt.Data.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<WishlistItem> WishlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseCollation("Arabic_CS_AI_KS_WS");

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Applicat__3214EC07CCCA7246");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            builder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC07728AB0EC");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ApplicationUser).WithMany(p => p.Carts).HasConstraintName("FK_Cart_ApplicationUser");
            });

            builder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC076E37658D");

                entity.Property(e => e.Quantity).HasDefaultValue(1);

                entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK_CartItem_Cart");

                entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartItem_Product");
            });

            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Category__3214EC070742BA3A");

                entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasConstraintName("FK_Category_Category");
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Order__3214EC071DA8F112");

                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ApplicationUser).WithMany(p => p.Orders)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_ApplicationUser");
            });

            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC070AD9229D");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK_OrderItem_Order");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_Product");
            });

            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC072BE14C30");

                entity.HasOne(d => d.Order).WithMany(p => p.Payments).HasConstraintName("FK_Payment_Order");
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Product__3214EC07E567E955");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(d => d.Category).WithMany(p => p.Products)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Seller");
            });

            builder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC0784A10355");

                entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasConstraintName("FK_ProductImage_Product");
            });

            builder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Review__3214EC0719AB872F");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ApplicationUser).WithMany(p => p.Reviews)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Review_ApplicationUser");

                entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasConstraintName("FK_Review_Product");
            });

            builder.Entity<WishlistItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Wishlist__3214EC07359C2F88");

                entity.HasOne(d => d.ApplicationUser).WithMany(p => p.WishlistItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WishlistItem_ApplicationUser");

                entity.HasOne(d => d.Product).WithMany(p => p.WishlistItems).HasConstraintName("FK_WishlistItem_Product");
            });
        }
    }
}
