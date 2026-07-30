using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Contacts;
using DynamicAppClass.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppClass.Infrastructure.Repositories;

public sealed class ContactConfigRepository(AppDbContext dbContext) : IContactConfigRepository
{
    public IQueryable<AllowedContact> GetConfig(int typeId)
        => dbContext.AllowedContacts.AsNoTracking().Where(contact => contact.ClassTypeId == typeId);

    public async Task AddAsync(AllowedContact contact, CancellationToken cancellationToken) =>
        await dbContext.AllowedContacts.AddAsync(contact, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public Task<AllowedContact?> GetByIdAsync(int contactId, CancellationToken cancellationToken)
        => dbContext.AllowedContacts
            .AsNoTracking()
            .FirstOrDefaultAsync(contact => contact.Id == contactId, cancellationToken);

    public void Remove(AllowedContact contact)
        => dbContext.AllowedContacts.Remove(contact);
}
