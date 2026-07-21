using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassStatusRepository(AppDbContext dbContext) : IClassStatusRepository
{
    public async Task<IReadOnlyList<ClassStatus>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.ClassStatuses
            .AsNoTracking()
            .OrderBy(classStatus => classStatus.Name)
            .ToListAsync(cancellationToken);

    public async Task<ClassStatus?> GetAsync(int id, CancellationToken cancellationToken) =>
        await dbContext.ClassStatuses.SingleOrDefaultAsync(classStatus => classStatus.Id == id, cancellationToken);

    public async Task AddAsync(ClassStatus classStatus, CancellationToken cancellationToken) =>
        await dbContext.ClassStatuses.AddAsync(classStatus, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
