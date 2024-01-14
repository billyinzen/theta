using MediatR;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.UpdateVenueById;

public record UpdateVenueByIdCommand(Guid Id, string EntityTag, string Name) : IRequest<Venue>;