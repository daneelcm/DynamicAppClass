using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassTypeFeatureRepository(AppDbContext dbContext) : IClassTypeFeatureRepository
{
    public async Task<IReadOnlyList<ClassTypeFeature>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.ClassTypeFeatures
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<ClassTypeFeature?> GetAsync(int id, CancellationToken cancellationToken) =>
        await dbContext.ClassTypeFeatures.SingleOrDefaultAsync(classTypeFeature => classTypeFeature.Id == id, cancellationToken);

    public async Task AddAsync(ClassTypeFeature classTypeFeature, CancellationToken cancellationToken) =>
        await dbContext.ClassTypeFeatures.AddAsync(classTypeFeature, cancellationToken);

    public void Update(ClassTypeFeature classTypeFeature) => dbContext.ClassTypeFeatures.Update(classTypeFeature);

    public async Task<ClassTypeFeature?> GetAsync(int typeId, int id, CancellationToken cancellationToken) =>
        await dbContext.ClassTypeFeatures.SingleOrDefaultAsync(classTypeFeature => classTypeFeature.ClassTypeId == typeId && classTypeFeature.FeatureId == id, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
