using Journaway.Application.Repositories;
using Journaway.Domain.ValueObjects;
using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Repositories;

public class EfTravellerRepository : ITravellerRepository
{
    private readonly OccupancyDbContext _db;
    
    public EfTravellerRepository(OccupancyDbContext db) => _db = db;

    public async Task<Guid?> FindTravellerAsync(
        HotelId hotelId,
        Guid travelGroupId,
        string surname,
        string firstName,
        DateOnly dateOfBirth,
        CancellationToken ct)
    {
        surname = surname.Trim();
        firstName = firstName.Trim();
        return await _db.Travellers.AsNoTracking()
            .Where(t => t.HotelId == hotelId.Value
                        && t.TravelGroupId == travelGroupId
                        && t.Surname == surname
                        && t.FirstName == firstName
                        && t.DateOfBirth == dateOfBirth)
            .Select(t => (Guid?)t.Id)
            .SingleOrDefaultAsync(ct);
    }
}