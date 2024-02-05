using FluentValidation;
using FluentValidation.Results;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.CreateVenue;

public class CreateVenueCommandHandlerTests
{
    private readonly ValidationResult _validationResult = Substitute.For<ValidationResult>();
    private readonly IValidator<CreateVenueCommand> _validator = Substitute.For<IValidator<CreateVenueCommand>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IVenueRepository _venueRepository = Substitute.For<IVenueRepository>();
    
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

        _venueRepository.CreateAsync(Arg.Any<Venue>())
            .Returns(true);

        _unitOfWork.Venues.Returns(_venueRepository);
            
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Should().BeEquivalentTo(expected);
    }
    
    // Private Methods

    private CreateVenueCommandHandler CreateSut()
        => new(_unitOfWork, _validator);

    private void SetupValidator(bool result)
    {
        _validationResult.IsValid.Returns(result);
        
        _validator.ValidateAsync(Arg.Any<CreateVenueCommand>())
            .Returns(_validationResult);
    }
}