using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassTypeFeatureRepository
{
    Task<IReadOnlyList<ClassTypeFeature>> ListAsync(CancellationToken cancellationToken);
    Task<ClassTypeFeature?> GetAsync(int id, CancellationToken cancellationToken);
    Task<ClassTypeFeature?> GetAsync(int typeId, int id, CancellationToken cancellationToken);
    Task AddAsync(ClassTypeFeature classTypeFeature, CancellationToken cancellationToken);
    void Update(ClassTypeFeature classTypeFeature);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
