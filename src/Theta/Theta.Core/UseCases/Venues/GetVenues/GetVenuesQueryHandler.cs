using MediatR;
using Theta.Data.Services;
using Theta.Domain.Features.Venues;

namespace Theta.Core.UseCases.Venues.GetVenues;

public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, IEnumerable<Venue>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initialize a new instance of the <see cref="GetVenuesQueryHandler"/> class
    /// </summary>
    /// <param name="unitOfWork"></param>
    public GetVenuesQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;
    
    /// <inheritdoc />
    public async Task<IEnumerable<Venue>> Handle(GetVenuesQuery query, CancellationToken cancellationToken)
        => await _unitOfWork.Venues.GetAllAsync(cancellationToken);
}
