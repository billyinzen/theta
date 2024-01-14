using FluentValidation;
using MediatR;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.CreateVenue;

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Venue>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateVenueCommand> _validator;

    /// <summary>
    /// Initialize a new instance of the <see cref="CreateVenueCommandHandler"/> class
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="validator"></param>
    public CreateVenueCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateVenueCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Venue> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var venue = new Venue(request.Name);

        // Discard the result, it will always return CommandResult.Success
        _ = await _unitOfWork.Venues.CreateAsync(venue, cancellationToken);
        _ = await _unitOfWork.SaveAsync(cancellationToken);
        
        return venue;
    }
}