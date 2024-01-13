using MediatR;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.CreateVenue;

public record CreateVenueCommand(string Name) : IRequest<Venue>;