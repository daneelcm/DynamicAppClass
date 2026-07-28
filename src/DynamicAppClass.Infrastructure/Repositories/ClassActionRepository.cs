using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassActionRepository(AppDbContext dbContext) : IClassActionRepository
{
    public async Task<IReadOnlyList<ClassAction>> ListAsync(CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassActions)
            .AsNoTracking()
            .OrderBy(classAction => classAction.Name)
            .ToListAsync(cancellationToken);

    public async Task<ClassAction?> GetAsync(int id, CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassActions).SingleOrDefaultAsync(classAction => classAction.Id == id, cancellationToken);

    public async Task AddAsync(ClassAction classAction, CancellationToken cancellationToken) =>
        await dbContext.ClassActions.AddAsync(classAction, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<ClassAction> IncludeGraph(IQueryable<ClassAction> query) =>
        query.Include(classAction => classAction.AssignClassField)
             .Include(classAction => classAction.ConditionClassField);
}
