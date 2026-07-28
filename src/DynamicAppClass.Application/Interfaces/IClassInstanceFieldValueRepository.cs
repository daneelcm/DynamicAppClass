using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassInstanceFieldValueRepository
{
    Task<IReadOnlyList<ClassInstanceFieldValue>> ListAsync(CancellationToken cancellationToken);
    Task<ClassInstanceFieldValue?> GetAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(ClassInstanceFieldValue classInstanceFieldValue, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
