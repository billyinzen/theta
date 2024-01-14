using FluentValidation;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;


namespace Theta.Core.UseCases.Venues.CreateVenue;

public class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(command => command.Name)
            .MinimumLength(VenueConstants.NameMinimumLength)
            .WithMessage(ErrorMessages.ValueTooShort)
            .MaximumLength(VenueConstants.NameMaximumLength)
            .WithMessage(ErrorMessages.ValueTooLong)
            .MustAsync(async (name, cancellation) 
                => await unitOfWork.Venues.IsNameUniqueAsync(name, cancellationToken: cancellation))
            .WithMessage(ErrorMessages.ValueNotUnique);
    }
}