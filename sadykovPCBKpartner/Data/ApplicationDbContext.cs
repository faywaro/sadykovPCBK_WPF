using Microsoft.EntityFrameworkCore;
using sadykovPCBKpartner.Models;

namespace sadykovPCBKpartner.Data
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// Подключение выполняется от имени роли app через Entity Framework Core + Npgsql.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Partner>     Partners     { get; set; } = null!;
        public DbSet<PartnerType> PartnerTypes { get; set; } = null!;
        public DbSet<Product>     Products     { get; set; } = null!;
        public DbSet<PartnerSale> PartnerSales { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения к PostgreSQL; роль app с паролем 123456789
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=sadykovpcbk;Username=app;Password=123456789;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Явно задаём схему app для всех сущностей
            modelBuilder.HasDefaultSchema("app");

            modelBuilder.Entity<PartnerType>(entity =>
            {
                entity.ToTable("partner_types", "app");
                entity.HasIndex(e => e.TypeName).IsUnique();
            });

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("partners", "app");
                entity.HasIndex(e => e.Inn).IsUnique();

                entity.HasOne(p => p.PartnerType)
                      .WithMany(t => t.Partners)
                      .HasForeignKey(p => p.TypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products", "app");
                entity.HasIndex(e => e.Article).IsUnique();
            });

            modelBuilder.Entity<PartnerSale>(entity =>
            {
                entity.ToTable("partner_sales", "app");

                entity.HasOne(s => s.Partner)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(s => s.PartnerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Product)
                      .WithMany(prod => prod.Sales)
                      .HasForeignKey(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
