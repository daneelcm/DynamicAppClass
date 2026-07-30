using DynamicAppClass.Domain.Entities.Contacts;

namespace DynamicAppClass.Application.Interfaces;

public interface IClassInstanceContactRepository
{
    Task<ClassInstanceContact?> GetByIdAsync(int contactId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClassInstanceContact>> GetAsync(int instanceId, CancellationToken cancellationToken);
    Task AddAsync(ClassInstanceContact contact, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Remove(ClassInstanceContact contact);
}
