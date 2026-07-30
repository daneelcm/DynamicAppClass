using DynamicAppClass.Domain.Entities.Contacts;

namespace DynamicAppClass.Application.Interfaces;

public interface IContactConfigRepository
{
    IQueryable<AllowedContact> GetConfig(int typeId);
    Task<AllowedContact?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(AllowedContact contact, CancellationToken cancellationToken);
    void Remove(AllowedContact contact);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
