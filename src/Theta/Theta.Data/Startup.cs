using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Theta.Data.Context;
using Theta.Data.Services;

namespace Theta.Data;

public static class Startup
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ThetaDbContext>(options => 
            options.UseSqlite(configuration.GetConnectionString("Sqlite")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}