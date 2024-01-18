using FluentValidation;
using FluentValidation.Results;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.CreateVenue;

public class CreateVenueCommandHandlerTests
{
    private readonly Mock<ValidationResult> _validationResult = new();
    private readonly Mock<IValidator<CreateVenueCommand>> _validator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IVenueRepository> _venueRepository = new();
    
    // Handle

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        const string name = "test venue name";
        var command = new CreateVenueCommand(name);
        
        SetupValidator(false);

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsVenue_WhenVenueCreated()
    {
        const string name = "test venue name";
        var command = new CreateVenueCommand(name);
        var expected = new Venue(name);
        
        SetupValidator(true);

        _venueRepository.Setup(r => 
                r.CreateAsync(It.IsAny<Venue>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);
            
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Should().BeEquivalentTo(expected);
    }
    
    // Private Methods

    private CreateVenueCommandHandler CreateSut()
        => new(_unitOfWork.Object, _validator.Object);

    private void SetupValidator(bool result)
    {
        _validationResult.SetupGet(v => v.IsValid).Returns(result);
        _validator.Setup(v => 
                v.ValidateAsync(It.IsAny<CreateVenueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_validationResult.Object);
    }
}