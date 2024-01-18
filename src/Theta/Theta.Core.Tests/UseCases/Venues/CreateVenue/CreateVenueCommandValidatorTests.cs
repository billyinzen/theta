using Theta.Core.Tests.Extensions;
using Theta.Core.UseCases;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.CreateVenue;

public class CreateVenueCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IVenueRepository> _venueRepository = new();
    
    // ValidateAsync

    [Fact]
    public async Task Validation_ShouldPass_WhenValidCommandProvided()
    {
        var command = GetCommand("valid name");

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenCommandNameIsDuplicate()
    {
        var command = GetCommand("valid name");

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueNotUnique);
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenCommandNameTooShort()
    {
        var command = GetCommand(new string('-', VenueConstants.NameMinimumLength - 1));

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueTooShort);
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenCommandNameTooLong()
    {
        var command = GetCommand(new string('-', VenueConstants.NameMaximumLength + 1));

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueTooLong);
    }
    
    // Private Methods

    private CreateVenueCommandValidator CreateSut()
        => new(_unitOfWork.Object);

    private CreateVenueCommand GetCommand(string? name = null)
        => new(Name: name ?? string.Empty);
}