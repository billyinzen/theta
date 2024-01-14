using MediatR;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.GetVenues;

public record GetVenuesQuery() : IRequest<IEnumerable<Venue>>;