using MediatR;
using Theta.Common.Exceptions;
using Theta.Common.Models;
using Theta.Core.UseCases.Venues.UpdateVenueById;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.RemoveVenueById;

public class RemoveVenueByIdCommandHandler : IRequestHandler<RemoveVenueByIdCommand, CommandResult>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initialize a new instance of the <see cref="UpdateVenueByIdCommandHandler"/> class
    /// </summary>
    /// <param name="unitOfWork"></param>
    public RemoveVenueByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CommandResult> Handle(RemoveVenueByIdCommand request, CancellationToken cancellationToken)
    {
        var venue = await _unitOfWork.Venues.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException(typeof(Venue), request.Id);

        if (!venue.CompareEtag(request.EntityTag))
            throw new ConflictException(typeof(Venue), request.Id, venue.EntityTag, request.EntityTag);

        await _unitOfWork.Venues.RemoveAsync(venue, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        return true;
    }
}