using FluentAssertions;
using Moq;
using Theta.Common.Exceptions;
using Theta.Core.UseCases.Venues.GetVenueById;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.GetVenueById;

public class GetVenueByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IVenueRepository> _venueRepository = new();
    
    // Handle

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenNoVenueFoundWithId()
    {
        _venueRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Venue);

        _unitOfWork.SetupGet(uow => uow.Venues).Returns(_venueRepository.Object);
        
        var sut = CreateSut();
        var action = () => sut.Handle(new GetVenueByIdQuery(Guid.NewGuid()), default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsVenue_WhenVenueFoundWithId()
    {
        var expected = new Venue("test")
        {
            Id = Guid.NewGuid()
        };
        
        _venueRepository.Setup(r => r.GetByIdAsync(It.Is<Guid>(g => g.Equals(expected.Id)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        _unitOfWork.SetupGet(uow => uow.Venues).Returns(_venueRepository.Object);
        
        var sut = CreateSut();
        var actual = await sut.Handle(new GetVenueByIdQuery(expected.Id), default);

        actual.Should().BeEquivalentTo(expected);
    }
    
    // Private Methods

    private GetVenueByIdQueryHandler CreateSut()
        => new(_unitOfWork.Object);
}