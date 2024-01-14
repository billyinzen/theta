using MediatR;
using Theta.Common.Models;

namespace Theta.Core.UseCases.Venues.RemoveVenueById;

public record RemoveVenueByIdCommand(Guid Id, string EntityTag) : IRequest<CommandResult>;