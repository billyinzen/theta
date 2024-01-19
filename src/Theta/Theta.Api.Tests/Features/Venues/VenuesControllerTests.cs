using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Theta.Api.Errors;
using Theta.Api.Features.Venues;
using Theta.Api.Features.Venues.DTOs;
using Theta.Common.Exceptions;
using Theta.Common.Helpers;
using Theta.Core.UseCases.Venues.GetVenueById;
using Theta.Core.UseCases.Venues.GetVenues;
using Theta.Domain.Features.Venues;

namespace Theta.Api.Tests.Features.Venues;

public class VenuesControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    
    private readonly IMapper _mapper;

    public VenuesControllerTests()
    {
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<VenuesProfile>())
            .CreateMapper();
    }
    
    // Get Venues

    [Fact]
    public async Task GetVenues_ReturnsOk_WhenVenuesFound()
    {
        var venues = new[]
        {
            new Venue("Venue One")
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UnixEpoch,
                ModifiedDate = DateTimeOffset.UnixEpoch
            },
            new Venue("Venue Two")
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UnixEpoch,
                ModifiedDate = DateTimeOffset.UnixEpoch
            }
        };

        var expected = _mapper.Map<List<VenueReadDto>>(venues);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenuesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venues);

        var sut = CreateSut();
        var response = await sut.GetVenues();

        var okResponse = response as OkObjectResult;
        okResponse.Should().NotBeNull();
        okResponse!.StatusCode.Should().Be(200);

        var content = okResponse.Value as IEnumerable<VenueReadDto>;
        content.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetVenues_ReturnsNoContent_WhenNoVenuesFound()
    {
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenuesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Venue>());

        var sut = CreateSut();
        var response = await sut.GetVenues();

        var noContentResponse = response as NoContentResult;
        noContentResponse.Should().NotBeNull();
        noContentResponse!.StatusCode.Should().Be(204);
    }
    
    [Fact]
    public async Task GetVenues_ReturnsInternalServerError_WhenExceptionHandled()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.MinValue);
        
        var exception = new ApplicationException("test exception");

        var expected = ApplicationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenuesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.GetVenues();

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse!.StatusCode.Should().Be(500);
        
        var content = objectResponse.Value as ApplicationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    // GetVenueById

    [Fact]
    public async Task GetVenueById_ReturnsVenue_WhenVenueFoundWithId()
    {
        var venue = new Venue("Test Venue")
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.MinValue,
            ModifiedDate = DateTimeOffset.MaxValue
        };

        var expected = _mapper.Map<VenueReadDto>(venue);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenueByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue);

        var sut = CreateSut();
        var response = await sut.GetVenueById(venue.Id);

        // Check response headers
        sut.ControllerContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(venue.EntityTag);
        
        var okResponse = response as OkObjectResult;
        okResponse.Should().NotBeNull();
        okResponse?.StatusCode.Should().Be(200);
        
        var content = okResponse?.Value as VenueReadDto;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetVenueById_ReturnsNotFound_WhenVenueNotFound()
    {
        DateTimeOffsetHelper.Set(new DateTimeOffset(2024, 1, 1, 12, 34, 15, TimeSpan.FromHours(5)));
        
        var exception = new NotFoundException(typeof(Venue), Guid.NewGuid());
        var expected = NotFoundErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenueByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.GetVenueById(exception.Id);

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(404);

        var content = objectResponse?.Value as NotFoundErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetVenueById_ReturnsInvalidServerError_WhenExceptionHandled()
    {
        DateTimeOffsetHelper.Set(new DateTimeOffset(2014, 5, 4, 16, 54, 37, TimeSpan.FromHours(-2)));
        
        var exception = new ApplicationException("Test exception");
        var expected = ApplicationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenueByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.GetVenueById(Guid.NewGuid());

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(500);

        var content = objectResponse?.Value as ApplicationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    // GetVenueByIdHeaders
    
    [Fact]
    public async Task GetVenueByIdHeaders_ReturnsNoContent_WhenVenueFoundWithId()
    {
        var venue = new Venue("Test Venue")
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.MinValue,
            ModifiedDate = DateTimeOffset.MaxValue
        };
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenueByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue);

        var sut = CreateSut();
        var response = await sut.GetVenueByIdHeaders(venue.Id);

        sut.ControllerContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(venue.EntityTag);
        
        var okResponse = response as NoContentResult;
        okResponse.Should().NotBeNull();
        okResponse?.StatusCode.Should().Be(204);
    }
    
    [Fact]
    public async Task GetVenueByIdGetVenueByIdHeaders_ReturnsNotFound_WhenVenueNotFound()
    {
        DateTimeOffsetHelper.Set(new DateTimeOffset(2024, 1, 1, 12, 34, 15, TimeSpan.FromHours(5)));
        
        var exception = new NotFoundException(typeof(Venue), Guid.NewGuid());
        var expected = NotFoundErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenueByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.GetVenueByIdHeaders(exception.Id);

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(404);

        var content = objectResponse?.Value as NotFoundErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetVenueByIdHeaders_ReturnsInvalidServerError_WhenExceptionHandled()
    {
        DateTimeOffsetHelper.Set(new DateTimeOffset(2014, 5, 4, 16, 54, 37, TimeSpan.FromHours(-2)));
        
        var exception = new ApplicationException("Test exception");
        var expected = ApplicationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<GetVenueByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.GetVenueByIdHeaders(Guid.NewGuid());

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(500);

        var content = objectResponse?.Value as ApplicationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    // Private methods

    private VenuesController CreateSut() => 
        new(_mediator.Object, _mapper)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}