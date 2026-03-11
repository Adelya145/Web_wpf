using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ApiParfum.Models;

public partial class DbParfumeContext : DbContext
{
    public DbParfumeContext()
    {
    }

    public DbParfumeContext(DbContextOptions<DbParfumeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bascet> Bascets { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderComposition> OrderCompositions { get; set; }

    public virtual DbSet<PickUpPoint> PickUpPoints { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<TovarCategory> TovarCategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=2703;database=db_parfume", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Bascet>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TovarArticle })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("bascet");

            entity.HasIndex(e => e.TovarArticle, "fk_t_article_idx");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TovarArticle)
                .HasMaxLength(6)
                .HasColumnName("tovar_article");
            entity.Property(e => e.BascetCount).HasColumnName("bascet_count");

            entity.HasOne(d => d.TovarArticleNavigation).WithMany(p => p.Bascets)
                .HasForeignKey(d => d.TovarArticle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_t_article");

            entity.HasOne(d => d.User).WithMany(p => p.Bascets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user1");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PRIMARY");

            entity.ToTable("manufacturers");

            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.ManufacturerName)
                .HasMaxLength(25)
                .HasColumnName("manufacturer_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.PickUpPointId, "fk_pick-up_idx");

            entity.HasIndex(e => e.UserId, "fk_user_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(4)
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderDateDelivery).HasColumnName("order_date_delivery");
            entity.Property(e => e.OrderStatus)
                .HasColumnType("enum('Завершен','Новый')")
                .HasColumnName("order_status");
            entity.Property(e => e.PickUpPointId).HasColumnName("pick-up_point_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PickUpPoint).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PickUpPointId)
                .HasConstraintName("fk_pick-up");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user");
        });

        modelBuilder.Entity<OrderComposition>(entity =>
        {
            entity.HasKey(e => e.OrderCompositionId).HasName("PRIMARY");

            entity.ToTable("order_composition");

            entity.HasIndex(e => e.OrderId, "fk_order_idx");

            entity.HasIndex(e => e.TovarArticle, "fk_tovar_idx");

            entity.Property(e => e.OrderCompositionId).HasColumnName("order_composition_id");
            entity.Property(e => e.OrderCompositionCount).HasColumnName("order_composition_count");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TovarArticle)
                .HasMaxLength(6)
                .HasColumnName("tovar_article");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderCompositions)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order");

            entity.HasOne(d => d.TovarArticleNavigation).WithMany(p => p.OrderCompositions)
                .HasForeignKey(d => d.TovarArticle)
                .HasConstraintName("fk_tovar");
        });

        modelBuilder.Entity<PickUpPoint>(entity =>
        {
            entity.HasKey(e => e.PickUpPointId).HasName("PRIMARY");

            entity.ToTable("pick-up_points");

            entity.Property(e => e.PickUpPointId).HasColumnName("pick-up_point_id");
            entity.Property(e => e.PickUpPointCity)
                .HasMaxLength(20)
                .HasColumnName("pick-up_point_city");
            entity.Property(e => e.PickUpPointHome)
                .HasMaxLength(5)
                .HasColumnName("pick-up_point_home");
            entity.Property(e => e.PickUpPointIndex)
                .HasMaxLength(6)
                .HasColumnName("pick-up_point_index");
            entity.Property(e => e.PickUpPointStreet)
                .HasMaxLength(50)
                .HasColumnName("pick-up_point_street");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PRIMARY");

            entity.ToTable("suppliers");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(20)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.TovarArticle).HasName("PRIMARY");

            entity.ToTable("tovars");

            entity.HasIndex(e => e.TovarCategoryId, "fk_category_idx");

            entity.HasIndex(e => e.ManufactrurerId, "fk_manufacturer_idx");

            entity.HasIndex(e => e.SupplierId, "fk_supplier_idx");

            entity.Property(e => e.TovarArticle)
                .HasMaxLength(6)
                .HasColumnName("tovar_article");
            entity.Property(e => e.ManufactrurerId).HasColumnName("manufactrurer_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TovarCategoryId).HasColumnName("tovar_category_id");
            entity.Property(e => e.TovarCost)
                .HasPrecision(10, 2)
                .HasColumnName("tovar_cost");
            entity.Property(e => e.TovarCount).HasColumnName("tovar_count");
            entity.Property(e => e.TovarCurrentSale).HasColumnName("tovar_current_sale");
            entity.Property(e => e.TovarDesc)
                .HasColumnType("text")
                .HasColumnName("tovar_desc");
            entity.Property(e => e.TovarName)
                .HasMaxLength(35)
                .HasColumnName("tovar_name");
            entity.Property(e => e.TovarPhoto)
                .HasMaxLength(45)
                .HasColumnName("tovar_photo");
            entity.Property(e => e.TovarUnit)
                .HasMaxLength(4)
                .HasColumnName("tovar_unit");

            entity.HasOne(d => d.Manufactrurer).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.ManufactrurerId)
                .HasConstraintName("fk_manufacturer");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_supplier");

            entity.HasOne(d => d.TovarCategory).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarCategoryId)
                .HasConstraintName("fk_category");
        });

        modelBuilder.Entity<TovarCategory>(entity =>
        {
            entity.HasKey(e => e.TovarCategoryId).HasName("PRIMARY");

            entity.ToTable("tovar_category");

            entity.Property(e => e.TovarCategoryId).HasColumnName("tovar_category_id");
            entity.Property(e => e.TovarCategoryName)
                .HasMaxLength(20)
                .HasColumnName("tovar_category_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserLastname)
                .HasMaxLength(30)
                .HasColumnName("user_lastname");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(55)
                .HasColumnName("user_login");
            entity.Property(e => e.UserName)
                .HasMaxLength(25)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .HasColumnName("user_password");
            entity.Property(e => e.UserRole)
                .HasMaxLength(50)
                .HasColumnName("user_role");
            entity.Property(e => e.UserSurname)
                .HasMaxLength(30)
                .HasColumnName("user_surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
