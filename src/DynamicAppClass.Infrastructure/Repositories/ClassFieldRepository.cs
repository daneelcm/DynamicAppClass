using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassFieldRepository(AppDbContext dbContext) : IClassFieldRepository
{
    public async Task<IReadOnlyList<ClassField>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.ClassFields
            .AsNoTracking()
            .OrderBy(classField => classField.Label)
            .ToListAsync(cancellationToken);

    public async Task<ClassField?> GetAsync(int id, CancellationToken cancellationToken) =>
        await dbContext.ClassFields.SingleOrDefaultAsync(classField => classField.Id == id, cancellationToken);

    public async Task AddAsync(ClassField classField, CancellationToken cancellationToken) =>
        await dbContext.ClassFields.AddAsync(classField, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
