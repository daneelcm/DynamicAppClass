using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassFieldRepository
{
    Task<IReadOnlyList<ClassField>> ListAsync(CancellationToken cancellationToken);
    Task<ClassField?> GetAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(ClassField classField, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
