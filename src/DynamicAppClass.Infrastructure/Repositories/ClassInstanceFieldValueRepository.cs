using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassInstanceFieldValueRepository(AppDbContext dbContext) : IClassInstanceFieldValueRepository
{
    public async Task<IReadOnlyList<ClassInstanceFieldValue>> ListAsync(CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassInstanceFieldValues)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<ClassInstanceFieldValue?> GetAsync(int id, CancellationToken cancellationToken) =>
        await IncludeGraph(dbContext.ClassInstanceFieldValues).SingleOrDefaultAsync(fieldValue => fieldValue.Id == id, cancellationToken);

    public async Task AddAsync(ClassInstanceFieldValue classInstanceFieldValue, CancellationToken cancellationToken) =>
        await dbContext.ClassInstanceFieldValues.AddAsync(classInstanceFieldValue, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<ClassInstanceFieldValue> IncludeGraph(IQueryable<ClassInstanceFieldValue> query) =>
        query.Include(fieldValue => fieldValue.ClassField);
}
