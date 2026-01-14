using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;

namespace Journaway.Application.UseCases;

public interface IAssignTravellerUseCase
{
    /// <summary>
    /// Assigns a traveller to a room on a given date.
    /// Returns null on success, otherwise an ApiError.
    /// </summary>
    Task<ApiError> AssignTraveller(AssignTravellerRequestDto request, CancellationToken ct);
}