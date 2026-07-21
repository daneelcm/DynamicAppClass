using DynamicAppClass.Domain.Entities;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassActionRepository
{
    Task<IReadOnlyList<ClassAction>> ListAsync(CancellationToken cancellationToken);
    Task<ClassAction?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ClassAction classAction, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
