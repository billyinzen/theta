using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Theta.Api.Errors;
using Theta.Api.Features.Venues;
using Theta.Api.Features.Venues.DTOs;
using Theta.Common.Exceptions;
using Theta.Common.Helpers;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Core.UseCases.Venues.GetVenueById;
using Theta.Core.UseCases.Venues.GetVenues;
using Theta.Core.UseCases.Venues.RemoveVenueById;
using Theta.Core.UseCases.Venues.UpdateVenueById;
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
            .ReturnsAsync(venues)
            .Verifiable(Times.Once);

        var sut = CreateSut();
        var response = await sut.GetVenues();

        var okResponse = response as OkObjectResult;
        okResponse.Should().NotBeNull();
        okResponse?.StatusCode.Should().Be(200);

        var content = okResponse?.Value as IEnumerable<VenueReadDto>;
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
        noContentResponse?.StatusCode.Should().Be(204);
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
        objectResponse?.StatusCode.Should().Be(500);
        
        var content = objectResponse?.Value as ApplicationErrorModel;
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

        var query = new GetVenueByIdQuery(venue.Id);

        var expected = _mapper.Map<VenueReadDto>(venue);
        
        _mediator.Setup(m =>
                m.Send(It.Is<GetVenueByIdQuery>(q => q == query), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue)
            .Verifiable(Times.Once);

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

        var query = new GetVenueByIdQuery(venue.Id);
        
        _mediator.Setup(m =>
                m.Send(It.Is<GetVenueByIdQuery>(q => q == query), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue)
            .Verifiable(Times.Once);

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

    // Add Venue

    [Fact]
    public async Task AddVenue_ReturnsCreated_WhenVenueCreated()
    {
        var command = new CreateVenueCommand("New test venue");

        var venue = new Venue(command.Name)
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.MinValue,
            ModifiedDate = DateTimeOffset.Now
        };

        var expected = _mapper.Map<VenueReadDto>(venue);
        
        _mediator.Setup(m =>
                m.Send(It.Is<CreateVenueCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue)
            .Verifiable(Times.Once);

        var sut = CreateSut();
        var response = await sut.AddVenue(new VenueWriteDto(command.Name));

        sut.ControllerContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(venue.EntityTag);

        var createdResponse = response as CreatedAtActionResult;
        createdResponse.Should().NotBeNull();
        createdResponse?.StatusCode.Should().Be(201);
        createdResponse?.RouteValues?["id"].Should().BeEquivalentTo(venue.Id);

        var content = createdResponse?.Value as VenueReadDto;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddVenue_ReturnsValidationError_WhenValidationFailed()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);

        var validationFailures = new[]
        {
            new ValidationFailure("Property_1", "Message_1"),
            new ValidationFailure("Property_1", "Message_2"),
            new ValidationFailure("Property_2", "Message_3"),
        };
        
        var exception = new ValidationException(validationFailures);
        var expected = ValidationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<CreateVenueCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.AddVenue(new VenueWriteDto("test name"));

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(400);

        var content = objectResponse?.Value as ValidationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddVenue_ReturnsApplicationError_WhenExceptionHandled()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new ApplicationException("Test exception");
        var expected = ApplicationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<CreateVenueCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.AddVenue(new VenueWriteDto("test name"));

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(500);

        var content = objectResponse?.Value as ApplicationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    // UpdateVenueById

    [Fact]
    public async Task UpdateVenueById_ReturnsOk_WhenVenueUpdate()
    {
        var venue = new Venue("updated value")
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.MinValue,
            ModifiedDate = DateTimeOffset.UnixEpoch
        };

        var expected = _mapper.Map<VenueReadDto>(venue);
        
        var command = new UpdateVenueByIdCommand(venue.Id, "input etag", "updated value");

        _mediator.Setup(m =>
                m.Send(It.Is<UpdateVenueByIdCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(venue)
            .Verifiable(Times.Once);

        var sut = CreateSut();
        var response = await sut.UpdateVenueById(command.Id, command.EntityTag, new VenueWriteDto(command.Name));

        sut.ControllerContext.HttpContext.Response.Headers.ETag.Should().BeEquivalentTo(venue.EntityTag);
        
        var okResponse = response as OkObjectResult;
        okResponse.Should().NotBeNull();
        okResponse?.StatusCode.Should().Be(200);

        var content = okResponse?.Value as VenueReadDto;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateVenueById_ReturnsValidationError_WhenValidationFailed()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);

        var validationFailures = new[]
        {
            new ValidationFailure("Property_1", "Message_1"),
            new ValidationFailure("Property_1", "Message_2"),
            new ValidationFailure("Property_2", "Message_3"),
        };
        
        var exception = new ValidationException(validationFailures);
        var expected = ValidationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<UpdateVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.UpdateVenueById(Guid.NewGuid(), string.Empty, new VenueWriteDto("test name"));

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(400);

        var content = objectResponse?.Value as ValidationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateVenueById_ReturnsNotFoundError_WhenVenueNotFound()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new NotFoundException(typeof(Venue), Guid.NewGuid());
        var expected = NotFoundErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<UpdateVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.UpdateVenueById(Guid.NewGuid(), string.Empty, new VenueWriteDto("test name"));

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(404);

        var content = objectResponse?.Value as NotFoundErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateVenueById_ReturnsPreconditionFailed_WhenEtagInvalid()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new ConflictException(typeof(Venue), Guid.NewGuid(), "expected", "provided");
        var expected = ConflictErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<UpdateVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.UpdateVenueById(Guid.NewGuid(), string.Empty, new VenueWriteDto("test name"));

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(412);

        var content = objectResponse?.Value as ConflictErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateVenueById_ReturnsInternalServerError_WhenExceptionHandled()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new ApplicationException("Test exception");
        var expected = ApplicationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<UpdateVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.UpdateVenueById(Guid.NewGuid(), string.Empty, new VenueWriteDto("test name"));

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(500);

        var content = objectResponse?.Value as ApplicationErrorModel;
        content.Should().BeEquivalentTo(expected);
    }
    
    // RemoveVenueById

    [Fact]
    public async Task RemoveVenueById_ReturnsNoContent_WhenVenueRemoved()
    {
        var command = new RemoveVenueByIdCommand(Guid.NewGuid(), "entity tag");
        
        _mediator.Setup(m => 
                m.Send(It.Is<RemoveVenueByIdCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);

        var sut = CreateSut();
        var response = await sut.RemoveVenueById(command.Id, command.EntityTag);

        var noContentResponse = response as NoContentResult;
        noContentResponse.Should().NotBeNull();
        noContentResponse?.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task RemoveVenueById_ReturnsNotFound_WhenVenueNotFound()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new NotFoundException(typeof(Venue), Guid.NewGuid());
        var expected = NotFoundErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<RemoveVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.RemoveVenueById(Guid.NewGuid(), string.Empty);

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(404);

        var content = objectResponse?.Value as NotFoundErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task RemoveVenueById_ReturnsPreconditionFailed_WhenEtagInvalid()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new ConflictException(typeof(Venue), Guid.NewGuid(), "expected", "provided");
        var expected = ConflictErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<RemoveVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.RemoveVenueById(Guid.NewGuid(), string.Empty);

        var objectResponse = response as ObjectResult;
        objectResponse.Should().NotBeNull();
        objectResponse?.StatusCode.Should().Be(412);

        var content = objectResponse?.Value as ConflictErrorModel;
        content.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task RemoveVenueById_ReturnsInternalServerError_WhenExceptionHandled()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        
        var exception = new ApplicationException("Test exception");
        var expected = ApplicationErrorModel.FromException(exception);
        
        _mediator.Setup(m =>
                m.Send(It.IsAny<RemoveVenueByIdCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var sut = CreateSut();
        var response = await sut.RemoveVenueById(Guid.NewGuid(), string.Empty);

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