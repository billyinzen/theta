using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Theta.Api.Features.Venues.DTOs;
using Theta.Core.UseCases.Venues.CreateVenue;

namespace Theta.Api.Features.Venues;

/// <summary>
/// Controller representing operations involving Venues
/// </summary>
public class VenuesController : ThetaController
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initialize a new instance od the <see cref="VenuesController"/> class
    /// </summary>
    /// <param name="mediator"></param>
    public VenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetVenues()
        => await Task.FromResult(Ok());
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVenueById(Guid id)
        => await Task.FromResult(Ok());
    
    [HttpHead("{id:guid}")]
    [ProducesResponseType<VenueReadDto>(200)]
    [ProducesResponseType<object>(400)]
    [ProducesResponseType<object>(404)]
    [ProducesResponseType<object>(500)]
    public async Task<IActionResult> GetVenueByIdHeaders(Guid id)
        => await Task.FromResult(Ok());
    
    /// <summary>
    /// Record a new venue
    /// </summary>
    /// <param name="writeDto">Data transfer object representing the venue to create</param>
    [HttpPost]
    [ProducesResponseType<VenueReadDto>(201)]
    [ProducesResponseType<object>(400)]
    [ProducesResponseType<object>(500)]
    public async Task<IActionResult> AddVenue([FromBody] VenueWriteDto writeDto)
    {
        // log
        try
        {
            var command = new CreateVenueCommand(writeDto.Name);
            var venue = await _mediator.Send(command);

            var readDto = new VenueReadDto
            {
                Id = venue.Id,
                Name = venue.Name,
                CreatedDate = venue.CreatedDate,
                ModifiedDate = venue.ModifiedDate
            };
            
            SetEntityTagHeader(venue.EntityTag);
            
            return CreatedAtAction(nameof(GetVenueById), new { id = venue.Id }, readDto);
        }
        catch (ValidationException ex)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateVenueById(Guid id, [FromHeader(Name = "If-Match")] string etag, [FromBody] VenueWriteDto dto)
        => await Task.FromResult(Ok());
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveVenueById(Guid id, [FromHeader(Name = "If-Match")] string etag)
        => await Task.FromResult(Ok());
}