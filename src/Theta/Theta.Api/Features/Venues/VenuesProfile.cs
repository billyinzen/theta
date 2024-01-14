using AutoMapper;
using Theta.Api.Features.Venues.DTOs;
using Theta.Domain.Features.Venues;

namespace Theta.Api.Features.Venues;

/// <summary>
/// Automapper profile class for Venues models
/// </summary>
public class VenuesProfile : Profile
{
    /// <summary>
    /// Initialize a new instance of the <see cref="VenuesProfile"/> class
    /// </summary>
    public VenuesProfile()
    {
        CreateMap<Venue, VenueReadDto>();
    }
}