using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Theta.Core.UseCases.Venues.CreateVenue;

namespace Theta.Core;

public static class Startup
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
        
        services.AddValidatorsFromAssemblyContaining<CreateVenueCommandValidator>();
        
        return services;
    }
}