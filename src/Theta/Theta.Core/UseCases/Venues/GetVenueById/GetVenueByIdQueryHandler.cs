using MediatR;
using Theta.Common.Exceptions;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.GetVenueById;

public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, Venue>
{
    private readonly IUnitOfWork _unitOfWork;
    
    /// <summary>
    /// Initialize a new instance of the <see cref="GetVenueByIdQueryHandler"/> class
    /// </summary>
    /// <param name="unitOfWork"></param>
    public GetVenueByIdQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;


    public async Task<Venue> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Venues.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(typeof(Venue), request.Id);
}