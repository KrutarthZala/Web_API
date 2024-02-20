using Microsoft.EntityFrameworkCore;

namespace Product_CRUD_Web_API.Models;

public partial class ProductApiContext : DbContext
{
    // Constructor that accepts DbContextOptions
    public ProductApiContext(DbContextOptions<ProductApiContext> options)
        : base(options)
    {
    }

    // DbSet property representing the "Products" table
    public virtual DbSet<Product> Products { get; set; }

    // DbSet property representing the "User" table
    public virtual DbSet<UserModel> User { get; set; }

    
    // Method for configuring the model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Configuration of Product
        // Configuration for the "Product" entity
        modelBuilder.Entity<Product>(entity =>
        {
            // Primary key configuration
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED39992C10");

            // Table name mapping
            entity.ToTable("Product");

            // Column property configurations
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductDescription).HasMaxLength(200);
            entity.Property(e => e.ProductName).HasMaxLength(50);
            entity.Property(e => e.ProductPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductImage).HasMaxLength(int.MaxValue);
        });
        #endregion

        #region Configuration of User
        modelBuilder.Entity<UserModel>(entity =>
        {
            // Primary key configuration
            entity.HasKey(e => e.UserID);

            // Table name mapping (optional if the table name matches the entity name)
            entity.ToTable("UserDetails");

            // Column property configurations
            entity.Property(e => e.UserID).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.UserEmail).HasMaxLength(50);
            entity.Property(e => e.UserPassword).HasMaxLength(50);
            entity.Property(e => e.UserJWTToken).HasMaxLength(int.MaxValue);
            entity.Property(e => e.JWTTokenIssueDate).HasColumnName("JWTTokenIssueDate");
            entity.Property(e => e.JWTTokenExpiryDate).HasColumnName("JWTTokenExpiryDate");
        });
        #endregion

        // Call a partial method for additional model configuration
        OnModelCreatingPartial(modelBuilder);
    }

    // Partial method for additional model configuration (can be implemented in another file)
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
