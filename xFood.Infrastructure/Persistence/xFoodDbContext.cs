using xFood.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace xFood.Infrastructure.Persistence
{
    // DbContext principal da aplicação.
    public class xFoodDbContext : DbContext
    {
        public xFoodDbContext(DbContextOptions<xFoodDbContext> options) : base(options)
        {
        }

        // Tabelas do banco.
        public DbSet<TypeUser> TypeUsers => Set<TypeUser>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        // Configurações via Fluent API.
        protected override void OnModelCreating(ModelBuilder mb)
        {
            // TYPEUSER
            mb.Entity<TypeUser>(e =>
            {
                e.ToTable("TypeUser");

                e.HasKey(x => x.Id);

                e.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(150);

                // 1:N — TypeUser -> Users
                e.HasMany(t => t.Users)
                 .WithOne(u => u.TypeUser)
                 .HasForeignKey(u => u.TypeUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // USER
            mb.Entity<User>(e =>
            {
                e.ToTable("User");

                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                e.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                e.Property(x => x.Password)
                    .IsRequired()
                    .HasMaxLength(150);

                e.Property(x => x.DateBirth)
                    .IsRequired();

                // Usuários ativos por padrão
                e.Property(x => x.Active)
                    .HasDefaultValue(true);
            });

            // CATEGORY
            mb.Entity<Category>(e =>
            {
                e.ToTable("Category");

                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(80);

                e.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                // Evita nomes duplicados
                e.HasIndex(x => x.Name).IsUnique();

                // 1:N — Category -> Products
                e.HasMany(c => c.Products)
                 .WithOne(p => p.Category)
                 .HasForeignKey(p => p.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // PRODUCT
            mb.Entity<Product>(e =>
            {
                e.ToTable("Product");

                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(120);

                e.Property(x => x.Description)
                    .HasMaxLength(1000);

                e.Property(x => x.Price)
                    .HasColumnType("decimal(18,2)")
                    .HasPrecision(18, 2);

                e.Property(x => x.Stock)
                    .IsRequired();

                e.Property(x => x.ImageUrl)
                    .HasMaxLength(2048);

                // Índices úteis
                e.HasIndex(x => x.Name);
                e.HasIndex(x => new { x.CategoryId, x.Name });
            });
        }
    }
}
