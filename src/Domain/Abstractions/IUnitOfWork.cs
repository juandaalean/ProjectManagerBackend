namespace Domain.Abstractions;

/// <summary>
/// Represents a unit of work for managing database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}