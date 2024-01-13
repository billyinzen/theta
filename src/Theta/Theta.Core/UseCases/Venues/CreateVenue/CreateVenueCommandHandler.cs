using FluentValidation;
using MediatR;
using Theta.Data.Repositories.Interfaces;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.CreateVenue;

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Venue>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<CreateVenueCommand> _validator;

    /// <summary>
    /// Initialize a new instance of the <see cref="CreateVenueCommandHandler"/> class
    /// </summary>
    /// <param name="venueRepository"></param>
    /// <param name="validator"></param>
    public CreateVenueCommandHandler(IVenueRepository venueRepository, IValidator<CreateVenueCommand> validator)
    {
        _venueRepository = venueRepository;
        _validator = validator;
    }

    public async Task<Venue> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var venue = new Venue(request.Name);

        // Discard the result, it will always return CommandResult.Success
        _ = await _venueRepository.CreateAsync(venue, cancellationToken);
        
        return venue;
    }
}