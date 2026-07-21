using DynamicAppClass.Domain.Entities;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassTypeRepository
{
    Task<IReadOnlyList<ClassType>> ListAsync(CancellationToken cancellationToken);
    Task<ClassType?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ClassType classType, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
