using FluentValidation;
using FluentValidation.Results;
using Theta.Common.Exceptions;
using Theta.Core.UseCases.Venues.UpdateVenueById;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.UpdateVenueById;

public class UpdateVenueByCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IVenueRepository _venueRepository = Substitute.For<IVenueRepository>();
    private readonly ValidationResult _validationResult = Substitute.For<ValidationResult>();
    private readonly IValidator<UpdateVenueByIdCommand> _validator = Substitute.For<IValidator<UpdateVenueByIdCommand>>();
    
    // Handle

    [Fact]
    public async Task Handle_ReturnsVenue_WhenVenueUpdated()
    {
        var id = Guid.NewGuid();
        const string initialName = "original venue";
        const string updateName = "updated venue";
        
        var venue = GetVenue(id, initialName);
        
        SetupRepository(venue);
        SetupValidator(true);

        var command = new UpdateVenueByIdCommand(id, venue.EntityTag, updateName);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Name.Should().BeEquivalentTo(updateName);
    }
    
    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFails()
    {
        var venue = GetVenue(Guid.NewGuid(), "name");
        
        SetupValidator(false);
        
        var command = new UpdateVenueByIdCommand(venue.Id, venue.EntityTag, venue.Name);
        
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);

        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenVenueNotFound()
    {
        var id = Guid.NewGuid();
        const string initialName = "original venue";
        const string updateName = "updated venue";
        
        var venue = GetVenue(id, initialName);
        
        SetupRepository(null);
        SetupValidator(true);

        var command = new UpdateVenueByIdCommand(id, venue.EntityTag, updateName);

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsConflictException_WhenEtagIncorrect()
    {
        var venue = GetVenue(Guid.NewGuid(), "test");
        
        SetupRepository(venue);
        SetupValidator(true);

        var command = new UpdateVenueByIdCommand(venue.Id, "invalid_etag", venue.Name);

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        
        await action.Should().ThrowAsync<ConflictException>();
    }
    
    // Private Methods

    private UpdateVenueByIdCommandHandler CreateSut()
        => new(_unitOfWork, _validator);

    private Venue GetVenue(Guid id, string name)
        => new(name)
        {
            Id = id,
            CreatedDate = DateTimeOffset.UnixEpoch,
            ModifiedDate = DateTimeOffset.UnixEpoch
        };
    
    private void SetupValidator(bool result)
    {
        _validationResult.IsValid.Returns(result);
        _validator.ValidateAsync(Arg.Any<UpdateVenueByIdCommand>())
            .Returns(_validationResult);
    }

    private void SetupRepository(Venue? venue)
    {
        _venueRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(venue);
        _unitOfWork.Venues.Returns(_venueRepository);
    }
}