using DynamicAppClass.Domain.Entities;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassFieldRepository
{
    Task<IReadOnlyList<ClassField>> ListAsync(CancellationToken cancellationToken);
    Task<ClassField?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ClassField classField, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
