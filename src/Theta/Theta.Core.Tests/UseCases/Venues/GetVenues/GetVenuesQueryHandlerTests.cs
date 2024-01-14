using FluentAssertions;
using Moq;
using Theta.Core.UseCases.Venues.GetVenues;
using Theta.Data.Repositories.Interfaces;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.Tests.UseCases.Venues.GetVenues;

public class GetVenuesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IVenueRepository> _venueRepository = new();

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

        _venueRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        
        _unitOfWork.SetupGet(uow => uow.Venues)
            .Returns(_venueRepository.Object);

        var sut = CreateSut();
        var result = await sut.Handle(new GetVenuesQuery(), default);

        result.Should().BeEquivalentTo(data);
    }
    
    // Private Methods

    private GetVenuesQueryHandler CreateSut()
        => new(_unitOfWork.Object);
}