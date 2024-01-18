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
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IVenueRepository> _venueRepository = new();
    private readonly Mock<ValidationResult> _validationResult = new();
    private readonly Mock<IValidator<UpdateVenueByIdCommand>> _validator = new();
    
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
        => new(_unitOfWork.Object, _validator.Object);

    private Venue GetVenue(Guid id, string name)
        => new(name)
        {
            Id = id,
            CreatedDate = DateTimeOffset.UnixEpoch,
            ModifiedDate = DateTimeOffset.UnixEpoch
        };
    
    private void SetupValidator(bool result)
    {
        _validationResult.SetupGet(v => v.IsValid).Returns(result);
        
        _validator.Setup(v => 
                v.ValidateAsync(It.IsAny<UpdateVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_validationResult.Object);
    }

    private void SetupRepository(Venue? venue)
    {
        _venueRepository.Setup(r => 
                r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue);

        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);
    }
}