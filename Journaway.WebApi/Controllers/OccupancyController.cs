using Journaway.Application.Dtos;
using Journaway.Application.Occupancy;
using Microsoft.AspNetCore.Mvc;

namespace Journaway.WebApi.Controllers;

public class OccupancyController: ControllerBase
{
    private readonly IOccupancyQueries _queries;

    public OccupancyController(IOccupancyQueries queries)
    {
        _queries = queries;
    }

    /// <summary>
    /// Hotels request all occupied rooms from today.
    /// </summary>
    [HttpGet("today")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OccupiedRoomDto>>> GetToday(
        [FromQuery] Guid hotelId,
        CancellationToken ct)
    {
        // Note: Using UTC date to avoid timezone surprises in a backend service.
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        var result = await _queries.GetOccupiedRoomsAsync(hotelId, today, ct);
        return Ok(result);
    }
    
    [HttpGet("date/{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<OccupiedRoomDto>>> GetByDate(
        [FromRoute] DateOnly date,
        [FromQuery] Guid hotelId,
        CancellationToken ct)
    {
        if (hotelId == Guid.Empty)
            return BadRequest(new { code = "invalid_request", message = "hotelId is required." });

        var result = await _queries.GetOccupiedRoomsAsync(hotelId, date, ct);
        return Ok(result);
    }

    /// <summary>
    /// Hotels request all rooms booked by a travel group for a date.
    /// </summary>
    [HttpGet("groups/{groupId}/rooms")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OccupiedRoomDto>>> GetRoomsByGroup(
        [FromRoute] string groupId,
        [FromQuery] Guid hotelId,
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var result = await _queries.GetRoomsByGroupAsync(hotelId, groupId, date, ct);
        return Ok(result);
    }

    /// <summary>
    /// Hotels request occupancy details for an individual room for a date.
    /// </summary>
    [HttpGet("rooms/{roomCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomOccupancyDto>> GetRoom(
        [FromRoute] string roomCode,
        [FromQuery] Guid hotelId,
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var result = await _queries.GetRoomOccupancyAsync(hotelId, roomCode, date, ct);
        return result is null ? NotFound() : Ok(result);
    }
}