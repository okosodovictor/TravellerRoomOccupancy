namespace Journaway.Application.Repositories;

public interface IUnitOfWork
{
    Task ExecuteSerializableAsync(
        Func<CancellationToken, Task> action,
        CancellationToken ct);
}