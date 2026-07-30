using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Contacts;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ClassInstanceContactRepository(AppDbContext dbContext) : IClassInstanceContactRepository
{
    public async Task<IReadOnlyList<ClassInstanceContact>> GetAsync(int instanceId, CancellationToken cancellationToken) =>
        await dbContext.ClassInstanceContacts
            .AsNoTracking()
            .Where(contact => contact.ClassInstanceId == instanceId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ClassInstanceContact contact, CancellationToken cancellationToken) =>
        await dbContext.ClassInstanceContacts.AddAsync(contact, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public Task<ClassInstanceContact?> GetByIdAsync(int contactId, CancellationToken cancellationToken)
        => dbContext.ClassInstanceContacts
            .AsNoTracking()
            .FirstOrDefaultAsync(contact => contact.Id == contactId, cancellationToken);

    public void Remove(ClassInstanceContact contact)
        => dbContext.ClassInstanceContacts.Remove(contact);
}
