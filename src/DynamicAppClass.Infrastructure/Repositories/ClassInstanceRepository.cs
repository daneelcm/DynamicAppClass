using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassInstanceRepository(AppDbContext dbContext) : IClassInstanceRepository
{
    public async Task<IReadOnlyList<ClassInstance>> ListAsync(CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassInstances)
            .AsNoTracking()
            .OrderByDescending(instance => instance.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<ClassInstance?> GetAsync(int id, CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassInstances).SingleOrDefaultAsync(instance => instance.Id == id, cancellationToken);

    public async Task AddAsync(ClassInstance instance, CancellationToken cancellationToken) =>
        await dbContext.ClassInstances.AddAsync(instance, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<ClassInstance> IncludeGraph(IQueryable<ClassInstance> query) =>
        query.Include(instance => instance.FieldValues);
}
