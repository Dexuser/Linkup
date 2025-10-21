using LinkUp.Core.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LinkUp.Core.Persistence
{
    public class LinkUpContextFactory : IDesignTimeDbContextFactory<LinkUpContext>
    {
        public LinkUpContext CreateDbContext(string[] args)
        {
            // Buscar el appsettings.json del proyecto WebApp (1 nivel arriba)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../LinkUp");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<LinkUpContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new LinkUpContext(optionsBuilder.Options);
        }
    }
}
