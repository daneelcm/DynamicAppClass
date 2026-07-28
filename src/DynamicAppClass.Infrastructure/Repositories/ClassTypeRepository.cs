using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassTypeRepository(AppDbContext dbContext) : IClassTypeRepository
{
    public async Task<IReadOnlyList<ClassType>> ListAsync(CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassTypes)
            .AsNoTracking()
            .OrderBy(classType => classType.Name)
            .ToListAsync(cancellationToken);

    public async Task<ClassType?> GetAsync(int id, CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassTypes).SingleOrDefaultAsync(classType => classType.Id == id, cancellationToken);

    public async Task AddAsync(ClassType classType, CancellationToken cancellationToken) =>
        await dbContext.ClassTypes.AddAsync(classType, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<ClassType> IncludeGraph(IQueryable<ClassType> query) =>
        query.Include(classType => classType.Fields)
                .ThenInclude(cf => cf.Field)
            .Include(classType => classType.Fields)
                .ThenInclude(cf => cf.Options)
                .ThenInclude(co => co.Lookup)
            .Include(classType => classType.Actions);
}
