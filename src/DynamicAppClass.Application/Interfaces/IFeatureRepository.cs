using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Application.Interfaces;

public interface IFeatureRepository
{
    Task<IReadOnlyList<Feature>> ListAsync(CancellationToken cancellationToken);
    Task<Feature?> GetAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Feature feature, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
