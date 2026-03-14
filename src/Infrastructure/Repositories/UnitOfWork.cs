using Application.Abstractions;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

/// <summary>
/// Unit of work implementation for managing database transactions.
/// </summary>
public class UnitOfWork(ProjectManagerContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}