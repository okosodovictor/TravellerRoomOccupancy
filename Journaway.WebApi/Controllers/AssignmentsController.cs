using Journaway.Application.Dtos;
using Journaway.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Journaway.WebApi.Controllers;

[ApiController]
[Route("api/assignments")]
public sealed class AssignmentsController: ControllerBase
{
    private readonly IMoveTravellerUseCase _moveTraveller;
    private readonly IAssignTravellerUseCase _assignTraveller;

    public AssignmentsController(
        IMoveTravellerUseCase moveTraveller,
        IAssignTravellerUseCase assignTraveller)
    {
        _moveTraveller = moveTraveller;
        _assignTraveller = assignTraveller;
    }
    
    [HttpPost("assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Assign([FromBody] AssignTravellerRequestDto request, CancellationToken ct)
    {
        var err = await _assignTraveller.AssignTraveller(request, ct);
        if (err is null)
            return NoContent();

        return err.Code switch
        {
            "invalid_request" => BadRequest(err),

            "group_not_found" => NotFound(err),
            "traveller_not_found" => NotFound(err),
            "room_not_found" => NotFound(err),

            "room_full" => Conflict(err),
            "already_assigned" => Conflict(err),

            _ => BadRequest(err)
        };
    }

    /// <summary>
    /// Move a traveller from one room to another for a given date.
    /// </summary>
    [HttpPost("move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Move([FromBody] MoveTravellerRequest request, CancellationToken ct)
    {
        var err = await _moveTraveller.ExecuteAsync(request, ct);
        if (err is null)
            return NoContent();

        return err.Code switch
        {
            "invalid_request" => BadRequest(err),

            "group_not_found" => NotFound(err),
            "traveller_not_found" => NotFound(err),
            "from_room_not_found" => NotFound(err),
            "to_room_not_found" => NotFound(err),
            "assignment_not_found" => NotFound(err),

            "wrong_source_room" => Conflict(err),
            "room_full" => Conflict(err),

            _ => BadRequest(err)
        };
    }
}