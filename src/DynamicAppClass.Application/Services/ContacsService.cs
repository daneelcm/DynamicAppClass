using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Contacts;

namespace DynamicAppClass.Application.Services;

public sealed class ContactService(IClassInstanceContactRepository classInstanceContacts)
{
    public async Task<IReadOnlyList<ClassInstanceContactDto>> GetInstanceContacts(int instanceId, CancellationToken cancellationToken)
        => [.. (await classInstanceContacts.GetAsync(instanceId, cancellationToken)).Select(MapToDto)];

    public async Task<ClassInstanceContactDto> CreateInstanceContact(AddContactRequest request, CancellationToken cancellationToken)
    {
        var contact = new ClassInstanceContact
        {
            ClassInstanceId = request.InstanceId, 
            FirstName = request.FirstName, 
            LastName = request.LastName, 
            ContactType = request.ContactType, 
            IsEntity = request.IsEntity, 
            Email = request.Email, 
            Phone = request.Phone, 
            EntityName = request.EntityName, 
            Address1 = request.Address1, 
            Address2 = request.Address2, 
            City = request.City, 
            ZipCode = request.ZipCode, 
            Country = request.Country, 
            LicenseNumber = request.LicenseNumber
        };
        await classInstanceContacts.AddAsync(contact, cancellationToken);
        await classInstanceContacts.SaveChangesAsync(cancellationToken);
        return MapToDto(contact);
    }


    public async Task RemoveInstanceContact(int contactId, CancellationToken cancellationToken)
    {
        var contact = await classInstanceContacts.GetByIdAsync(contactId, cancellationToken)
            ?? throw new InvalidOperationException($"Contact with id {contactId} not found.");

        classInstanceContacts.Remove(contact);
        await classInstanceContacts.SaveChangesAsync(cancellationToken);
    }

    private static ClassInstanceContactDto MapToDto(ClassInstanceContact contact)
        => new(contact.Id, contact.FirstName, contact.LastName, contact.ContactType, contact.IsEntity, contact.Email, contact.Phone, contact.EntityName, contact.Address1, contact.Address2, contact.City, contact.ZipCode, contact.Country, contact.LicenseNumber);
}