using Theta.Common.Exceptions;
using Theta.Common.Models;
using Theta.Core.UseCases.Venues.RemoveVenueById;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.RemoveVenueById;

public class RemoveVenueByIdCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IVenueRepository _venueRepository = Substitute.For<IVenueRepository>();
    
    // Handle

    [Fact]
    public async Task Handle_ReturnsCommandResult_WhenVenueRemoved()
    {
        var id = Guid.NewGuid();
        var venue = GetVenue(id);
        SetupRepository(venue);

        var command = new RemoveVenueByIdCommand(venue.Id, venue.EntityTag);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Should().BeEquivalentTo<CommandResult>(true);
    }
    
    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenVenueNotFound()
    {
        var id = Guid.NewGuid();
        var venue = GetVenue(id);
        SetupRepository(null);

        var command = new RemoveVenueByIdCommand(venue.Id, venue.EntityTag);

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsConflictException_WhenEtagIncorrect()
    {
        var id = Guid.NewGuid();
        var venue = GetVenue(id);
        SetupRepository(venue);

        var command = new RemoveVenueByIdCommand(venue.Id, "invalid etag");

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ConflictException>();
    }
    
    // Private Methods

    private RemoveVenueByIdCommandHandler CreateSut()
        => new(_unitOfWork);

    private Venue GetVenue(Guid id)
        => new("test-name")
        {
            Id = id,
            CreatedDate = DateTimeOffset.UnixEpoch,
            ModifiedDate = DateTimeOffset.UnixEpoch
        };

    private void SetupRepository(Venue? venue)
    {
        _venueRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(venue);

        _venueRepository.RemoveAsync(Arg.Any<Venue>())
            .Returns(true);

        _unitOfWork.Venues.Returns(_venueRepository);
    }
}