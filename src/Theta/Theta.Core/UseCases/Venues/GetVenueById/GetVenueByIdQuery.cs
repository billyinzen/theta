using MediatR;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.GetVenueById;

public record GetVenueByIdQuery(Guid Id) : IRequest<Venue>;