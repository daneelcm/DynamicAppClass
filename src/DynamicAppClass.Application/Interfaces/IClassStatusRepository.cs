using DynamicAppClass.Domain.Entities;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassStatusRepository
{
    Task<IReadOnlyList<ClassStatus>> ListAsync(CancellationToken cancellationToken);
    Task<ClassStatus?> GetAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(ClassStatus classStatus, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
