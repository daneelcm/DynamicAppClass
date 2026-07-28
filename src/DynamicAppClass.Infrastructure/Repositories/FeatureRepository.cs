using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class FeatureRepository(AppDbContext dbContext) : IFeatureRepository
{
    public async Task<IReadOnlyList<Feature>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Features
            .AsNoTracking()
            .OrderBy(feature => feature.Name)
            .ToListAsync(cancellationToken);

    public async Task<Feature?> GetAsync(int id, CancellationToken cancellationToken) =>
        await dbContext.Features.SingleOrDefaultAsync(feature => feature.Id == id, cancellationToken);

    public async Task AddAsync(Feature feature, CancellationToken cancellationToken) =>
        await dbContext.Features.AddAsync(feature, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
