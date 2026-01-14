using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;

namespace Journaway.Application.UseCases;

public interface IMoveTravellerUseCase
{
    // <summary>
    /// Moves a traveller from one room to another on a given date.
    /// Returns null on success, otherwise an ApiError.
    /// </summary>
    Task<ApiError?> ExecuteAsync(MoveTravellerRequest request, CancellationToken ct);
}