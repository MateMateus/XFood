using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace xFood.Infrastructure.Persistence
{
    // Fábrica usada pelo EF Core durante o tempo de design (migrações).
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<xFoodDbContext>
    {
        // Cria o DbContext usando a mesma conexão do appsettings.
        public xFoodDbContext CreateDbContext(string[] args)
        {
            // String de conexão usada para gerar migrações.
            var conn = "Server=(localdb)\\MSSQLLocalDB;Database=xFoodDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=true";

            var opts = new DbContextOptionsBuilder<xFoodDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new xFoodDbContext(opts);
        }
    }
}
