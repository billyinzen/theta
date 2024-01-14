using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Theta.Api.Errors;
using Theta.Api.Features.Venues.DTOs;
using Theta.Common.Exceptions;
using Theta.Core.UseCases.Venues.CreateVenue;
using Theta.Core.UseCases.Venues.GetVenueById;
using Theta.Core.UseCases.Venues.GetVenues;
using Theta.Core.UseCases.Venues.RemoveVenueById;
using Theta.Core.UseCases.Venues.UpdateVenueById;

namespace Theta.Api.Features.Venues;

/// <summary>
/// Controller representing operations involving Venues
/// </summary>
public class VenuesController : ThetaController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialize a new instance od the <see cref="VenuesController"/> class
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="mapper"></param>
    public VenuesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Get a list of venues
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<VenueReadDto>>(200)]
    [ProducesResponseType(204)]
    [ProducesResponseType<ApplicationErrorModel>(500)]
    public async Task<IActionResult> GetVenues()
    {
        try
        {
            var venues = await _mediator.Send(new GetVenuesQuery());
            
            if (!venues.Any())
                return NoContent();

            return Ok(_mapper.Map<IEnumerable<VenueReadDto>>(venues));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApplicationErrorModel.FromException(ex));
        }
    }
    
    /// <summary>
    /// Get a venue by its unique identifier
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<VenueReadDto>(200)]
    [ProducesResponseType<NotFoundErrorModel>(404)]
    [ProducesResponseType<ApplicationErrorModel>(500)]
    public async Task<IActionResult> GetVenueById(Guid id)
    {
        try
        {
            var venue = await _mediator.Send(new GetVenueByIdQuery(id));
            
            SetEntityTagHeader(venue.EntityTag);
            
            return Ok(_mapper.Map<VenueReadDto>(venue));
        }
        catch (NotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, NotFoundErrorModel.FromException(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApplicationErrorModel.FromException(ex));
        }
    }
    
    /// <summary>
    /// Get the header values for a venue by its unique identifier
    /// </summary>
    /// <param name="id"></param>
    [HttpHead("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<object>(404)]
    [ProducesResponseType<object>(500)]
    public async Task<IActionResult> GetVenueByIdHeaders(Guid id)
    {
        try
        {
            var venue = await _mediator.Send(new GetVenueByIdQuery(id));
            
            SetEntityTagHeader(venue.EntityTag);

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, NotFoundErrorModel.FromException(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApplicationErrorModel.FromException(ex));
        }
    }
    
    /// <summary>
    /// Add a new venue
    /// </summary>
    /// <param name="writeDto">Data transfer object representing the venue to create</param>
    [HttpPost]
    [ProducesResponseType<VenueReadDto>(201)]
    [ProducesResponseType<ValidationErrorModel>(400)]
    [ProducesResponseType<ApplicationErrorModel>(500)]
    public async Task<IActionResult> AddVenue([FromBody] VenueWriteDto writeDto)
    {
        try
        {
            var command = new CreateVenueCommand(writeDto.Name);
            var venue = await _mediator.Send(command);
            
            SetEntityTagHeader(venue.EntityTag);
            
            return CreatedAtAction(nameof(GetVenueById), new { id = venue.Id }, _mapper.Map<VenueReadDto>(venue));
        }
        catch (ValidationException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ValidationErrorModel.FromException(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApplicationErrorModel.FromException(ex));
        }
    }

    /// <summary>
    /// Update a venue
    /// </summary>
    /// <param name="id"></param>
    /// <param name="etag"></param>
    /// <param name="writeDto"></param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<VenueReadDto>(200)]
    [ProducesResponseType<ValidationErrorModel>(400)]
    [ProducesResponseType<NotFoundErrorModel>(404)]
    [ProducesResponseType<ConflictErrorModel>(412)]
    [ProducesResponseType<ApplicationErrorModel>(500)]
    public async Task<IActionResult> UpdateVenueById(Guid id, [FromHeader(Name = "If-Match")] string etag,
        [FromBody] VenueWriteDto writeDto)
    {
        try
        {
            var command = new UpdateVenueByIdCommand(id, etag, writeDto.Name);
            var venue = await _mediator.Send(command);
            
            SetEntityTagHeader(venue.EntityTag);
            
            return Ok(_mapper.Map<VenueReadDto>(venue));
        }
        catch (ValidationException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ValidationErrorModel.FromException(ex));
        }
        catch (NotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, NotFoundErrorModel.FromException(ex));
        }
        catch (ConflictException ex)
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed, ConflictErrorModel.FromException(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApplicationErrorModel.FromException(ex));
        }
    }

    /// <summary>
    /// Remove a venue
    /// </summary>
    /// <param name="id"></param>
    /// <param name="etag"></param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<NotFoundErrorModel>(404)]
    [ProducesResponseType<ConflictErrorModel>(412)]
    [ProducesResponseType<ApplicationErrorModel>(500)]
    public async Task<IActionResult> RemoveVenueById(Guid id, [FromHeader(Name = "If-Match")] string etag)
    {
        try
        {
            var command = new RemoveVenueByIdCommand(id, etag);
            _ = await _mediator.Send(command);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, NotFoundErrorModel.FromException(ex));
        }
        catch (ConflictException ex)
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed, ConflictErrorModel.FromException(ex));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApplicationErrorModel.FromException(ex));
        }
    }
}