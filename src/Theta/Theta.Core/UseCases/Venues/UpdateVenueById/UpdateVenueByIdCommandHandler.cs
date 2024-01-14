using FluentValidation;
using MediatR;
using Theta.Common.Exceptions;
using Theta.Common.Helpers;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.UpdateVenueById;

public class UpdateVenueByIdCommandHandler : IRequestHandler<UpdateVenueByIdCommand, Venue>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateVenueByIdCommand> _validator;

    /// <summary>
    /// Initialize a new instance of the <see cref="UpdateVenueByIdCommandHandler"/> class
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="validator"></param>
    public UpdateVenueByIdCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateVenueByIdCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Venue> Handle(UpdateVenueByIdCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var venue = await _unitOfWork.Venues.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(typeof(Venue), request.Id);

        if (!venue.CompareEtag(request.EntityTag))
            throw new ConflictException(typeof(Venue), request.Id, venue.EntityTag, request.EntityTag);

        // TODO: move this mutation into the domain layer somehow
        venue.Name = request.Name;

        await _unitOfWork.Venues.UpdateAsync(venue, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        
        return venue;
    }
}