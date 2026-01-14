using System.Data;
using Journaway.Application.Repositories;
using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Journaway.Infrastructure.Repositories;

public class EfUnitOfWork: IUnitOfWork
{
    private readonly OccupancyDbContext _db;

    public EfUnitOfWork(OccupancyDbContext db) => _db = db;

    public async Task ExecuteSerializableAsync(Func<CancellationToken, Task> action, CancellationToken ct)
    {
        // If you ever call UoW inside another UoW, reuse current transaction.
        var hasTx = _db.Database.CurrentTransaction is not null;

        if (hasTx)
        {
            await action(ct);
            return;
        }

        await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);

        try
        {
            await action(ct);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}