using Theta.Core.Tests.Extensions;
using Theta.Core.UseCases;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.CreateVenue;

public class CreateVenueCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IVenueRepository _venueRepository = Substitute.For<IVenueRepository>();
    
    // ValidateAsync

    [Fact]
    public async Task Validation_ShouldPass_WhenValidCommandProvided()
    {
        var command = GetCommand("valid name");

        _venueRepository.IsNameUniqueAsync(command.Name)
            .Returns(true);

        _unitOfWork.Venues.Returns(_venueRepository);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenCommandNameIsDuplicate()
    {
        var command = GetCommand("valid name");
    
        _venueRepository.IsNameUniqueAsync(command.Name)
            .Returns(false);

        _unitOfWork.Venues.Returns(_venueRepository);
    
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueNotUnique);
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenCommandNameTooShort()
    {
        var command = GetCommand(new string('-', VenueConstants.NameMinimumLength - 1));
    
        _venueRepository.IsNameUniqueAsync(command.Name)
            .Returns(true);

        _unitOfWork.Venues.Returns(_venueRepository);
    
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueTooShort);
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenCommandNameTooLong()
    {
        var command = GetCommand(new string('-', VenueConstants.NameMaximumLength + 1));
    
        _venueRepository.IsNameUniqueAsync(command.Name)
            .Returns(true);

        _unitOfWork.Venues.Returns(_venueRepository);
    
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueTooLong);
    }
    
    // Private Methods

    private CreateVenueCommandValidator CreateSut()
        => new(_unitOfWork);

    private static CreateVenueCommand GetCommand(string? name = null)
        => new(Name: name ?? string.Empty);
}