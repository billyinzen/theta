using Theta.Core.UseCases.Venues.GetVenues;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.GetVenues;

public class GetVenuesQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IVenueRepository _venueRepository = Substitute.For<IVenueRepository>();

    // Handle

    [Fact]
    public async Task Handle_ReturnsAllVenues()
    {
        var data = new[]
        {
            new Venue("One"),
            new Venue("Two"),
            new Venue("Three")
        };

        _venueRepository.GetAllAsync().Returns(data);
        _unitOfWork.Venues.Returns(_venueRepository);

        var sut = CreateSut();
        var result = await sut.Handle(new GetVenuesQuery(), default);

        result.Should().BeEquivalentTo(data);
    }
    
    // Private Methods

    private GetVenuesQueryHandler CreateSut()
        => new(_unitOfWork);
}