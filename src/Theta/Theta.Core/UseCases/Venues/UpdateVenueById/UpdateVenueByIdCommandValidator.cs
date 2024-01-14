using FluentValidation;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.UpdateVenueById;

public class UpdateVenueByIdCommandValidator : AbstractValidator<UpdateVenueByIdCommand>
{
    public UpdateVenueByIdCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(command => command.Name)
            .MinimumLength(VenueConstants.NameMinimumLength)
            .WithMessage(ErrorMessages.ValueTooShort)
            .MaximumLength(VenueConstants.NameMaximumLength)
            .WithMessage(ErrorMessages.ValueTooLong);

        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken)
                => await unitOfWork.Venues.IsNameUniqueAsync(command.Name, command.Id, cancellationToken))
            .WithName(nameof(UpdateVenueByIdCommand.Name))
            .WithMessage(ErrorMessages.ValueNotUnique);
    }
}