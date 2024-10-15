using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BaseballBandit.Models;

public partial class BaseballBanditContext : DbContext
{
    public BaseballBanditContext()
    {
    }

    public BaseballBanditContext(DbContextOptions<BaseballBanditContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<OrderLog> OrderLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Cart");

            entity.Property(e => e.FkUserId).HasColumnName("FkUserID");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Inventory");

            entity.Property(e => e.ProductApparelSize).HasMaxLength(50);
            entity.Property(e => e.ProductColor).HasMaxLength(50);
            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProductID");
            entity.Property(e => e.ProductType).HasMaxLength(50);
        });

        modelBuilder.Entity<OrderLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("OrderLog");

            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNum).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("User");

            entity.Property(e => e.AddressCity).HasMaxLength(50);
            entity.Property(e => e.AddressState).HasMaxLength(50);
            entity.Property(e => e.HashedPass).HasColumnName("hashedPass");
            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
