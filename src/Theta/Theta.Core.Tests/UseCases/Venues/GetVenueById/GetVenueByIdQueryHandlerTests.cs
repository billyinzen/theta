using Theta.Common.Exceptions;
using Theta.Core.UseCases.Venues.GetVenueById;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.GetVenueById;

public class GetVenueByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IVenueRepository _venueRepository = Substitute.For<IVenueRepository>();
    
    // Handle

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenNoVenueFoundWithId()
    {
        _venueRepository.GetByIdAsync(Arg.Any<Guid>())
            .Returns(null as Venue);

        _unitOfWork.Venues.Returns(_venueRepository);
        
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
        
        _venueRepository.GetByIdAsync(expected.Id)
            .Returns(expected);

        _unitOfWork.Venues.Returns(_venueRepository);
        
        var sut = CreateSut();
        var actual = await sut.Handle(new GetVenueByIdQuery(expected.Id), default);

        actual.Should().BeEquivalentTo(expected);
    }
    
    // Private Methods

    private GetVenueByIdQueryHandler CreateSut()
        => new(_unitOfWork);
}