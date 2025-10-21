using LinkUp.Core.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace LinkUp.Core.Persistence;

public static class ServicesRegistration
{
    // Extension method - Decorator pattern
    public static IServiceCollection AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<LinkUpContext>(
            opt =>
            {
                opt.EnableSensitiveDataLogging();
                opt.UseSqlServer(connectionString,
                    m => m.MigrationsAssembly(typeof(AppContext).Assembly.FullName));
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped
        );
        
        return services;
    }
}