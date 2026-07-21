using DynamicAppClass.Domain.Entities;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassInstanceRepository
{
    Task<IReadOnlyList<ClassInstance>> ListAsync(CancellationToken cancellationToken);
    Task<ClassInstance?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(ClassInstance instance, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
