using Theta.Core.Tests.Extensions;
using Theta.Core.UseCases;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Core.UseCases.Venues.UpdateVenueById;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.UpdateVenueById;

public class UpdateVenueCommandValidatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IVenueRepository> _venueRepository = new();
    
    // ValidateAsync

    [Fact]
    public async Task Validation_ShouldPass_WhenValidCommandProvided()
    {
        var command = GetCommand(name: "valid name");

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenNameIsDuplicate()
    {
        var command = GetCommand(name: "valid name");

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueNotUnique);
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenNameTooShort()
    {
        var command = GetCommand(name: new string('-', VenueConstants.NameMinimumLength - 1));

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueTooShort);
    }
    
    [Fact]
    public async Task Validation_ShouldFail_WhenNameTooLong()
    {
        var command = GetCommand(name: new string('-', VenueConstants.NameMaximumLength + 1));

        _venueRepository.Setup(vr => 
                vr.IsNameUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.ShouldHaveOneError(nameof(CreateVenueCommand.Name), ErrorMessages.ValueTooLong);
    }
    
    // Private Methods

    private UpdateVenueByIdCommandValidator CreateSut()
        => new(_unitOfWork.Object);

    private static UpdateVenueByIdCommand GetCommand(
        Guid? id = null,
        string? etag = null,
        string? name = null)
        => new(
            Id: id ?? Guid.NewGuid(),
            EntityTag: etag ?? string.Empty,
            Name: name ?? string.Empty);
}