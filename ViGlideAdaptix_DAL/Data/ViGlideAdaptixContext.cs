using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ViGlideAdaptix_DAL.Models;

public partial class ViGlideAdaptixContext : DbContext
{
    public ViGlideAdaptixContext()
    {
    }

    public ViGlideAdaptixContext(DbContextOptions<ViGlideAdaptixContext> options)
        : base(options)
    {
    }

    public virtual DbSet<About> Abouts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Privacy> Privacies { get; set; }

    public virtual DbSet<Product> Products { get; set; }

	public virtual DbSet<Brand> Brands { get; set; }

	public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<About>(entity =>
		{
			entity.ToTable("About");

			entity.Property(e => e.Detail).HasMaxLength(255);
		});

		modelBuilder.Entity<Brand>(entity =>
		{
			entity.ToTable("Brand");

			entity.Property(e => e.BrandName).HasMaxLength(50);
		});

		modelBuilder.Entity<Category>(entity =>
		{
			entity.ToTable("Category");

			entity.Property(e => e.CategoryName).HasMaxLength(100);
		});

		modelBuilder.Entity<Customer>(entity =>
		{
			entity.ToTable("Customer");

			entity.Property(e => e.Email)
				.HasMaxLength(100)
				.IsFixedLength();
			entity.Property(e => e.IsBanned).HasColumnName("isBanned");
			entity.Property(e => e.Password)
				.HasMaxLength(255)
				.IsFixedLength();
			entity.Property(e => e.PhoneNumber)
				.HasMaxLength(12)
				.IsUnicode(false)
				.IsFixedLength();

			entity.HasOne(d => d.Role).WithMany(p => p.Customers)
				.HasForeignKey(d => d.RoleId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Customer_Role");
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.ToTable("Order");

			entity.Property(e => e.OrderId).ValueGeneratedNever();
			entity.Property(e => e.CreatedDate).HasColumnType("datetime");
			entity.Property(e => e.EstDeliveryDate).HasColumnType("datetime");
			entity.Property(e => e.RealDeliveryDate).HasColumnType("datetime");
			entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

			entity.HasOne(d => d.Cart).WithMany(p => p.Orders)
				.HasForeignKey(d => d.CartId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Order_ShoppingCart");

			entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
				.HasForeignKey(d => d.CustomerId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Order_Customer");

			entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
				.HasForeignKey(d => d.PaymentMethodId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Order_PaymentMethod");
		});

		modelBuilder.Entity<OrderDetail>(entity =>
		{
			entity.HasKey(e => e.OrderItemId);

			entity.ToTable("OrderDetail");

			entity.Property(e => e.OrderItemId).ValueGeneratedNever();
			entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");
			entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

			entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
				.HasForeignKey(d => d.OrderId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_OrderDetail_Order");

			entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
				.HasForeignKey(d => d.ProductId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_OrderDetail_Product");
		});

		modelBuilder.Entity<PaymentMethod>(entity =>
		{
			entity.ToTable("PaymentMethod");

			entity.Property(e => e.PaymentMethodName).HasMaxLength(255);
		});

		modelBuilder.Entity<Privacy>(entity =>
		{
			entity.ToTable("Privacy");

			entity.Property(e => e.Detail).HasMaxLength(255);
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.ToTable("Product");

			entity.Property(e => e.ProductDescription).HasMaxLength(255);
			entity.Property(e => e.ProductImage).HasMaxLength(255);
			entity.Property(e => e.ProductName).HasMaxLength(100);
			entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

			entity.HasOne(d => d.Brand).WithMany(p => p.Products)
				.HasForeignKey(d => d.BrandId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Product_Brand");

			entity.HasOne(d => d.Category).WithMany(p => p.Products)
				.HasForeignKey(d => d.CategoryId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Product_Category");
		});

		modelBuilder.Entity<Rating>(entity =>
		{
			entity.ToTable("Rating");

			entity.Property(e => e.Comment).HasMaxLength(255);
			entity.Property(e => e.RatingDate).HasColumnType("datetime");

			entity.HasOne(d => d.Customer).WithMany(p => p.Ratings)
				.HasForeignKey(d => d.CustomerId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Rating_Customer");

			entity.HasOne(d => d.OrderItem).WithMany(p => p.Ratings)
				.HasForeignKey(d => d.OrderItemId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Rating_OrderItem");
		});

		modelBuilder.Entity<Role>(entity =>
		{
			entity.ToTable("Role");

			entity.Property(e => e.RoleName)
				.HasMaxLength(50)
				.IsFixedLength();
		});

		modelBuilder.Entity<ShoppingCart>(entity =>
		{
			entity.HasKey(e => e.CartId);

			entity.ToTable("ShoppingCart");

			entity.Property(e => e.CreatedDate).HasColumnType("datetime");
			entity.Property(e => e.ShippingPrice).HasColumnType("decimal(18, 0)");
			entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 0)");
			entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

			entity.HasOne(d => d.Customer).WithMany(p => p.ShoppingCarts)
				.HasForeignKey(d => d.CustomerId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_ShoppingCart_Customer");
		});

		modelBuilder.Entity<ShoppingCartItem>(entity =>
		{
			entity.HasKey(e => e.CartItemId);

			entity.ToTable("ShoppingCartItem");

			entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

			entity.HasOne(d => d.Cart).WithMany(p => p.ShoppingCartItems)
				.HasForeignKey(d => d.CartId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_ShoppingCartItem_ShoppingCart");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
