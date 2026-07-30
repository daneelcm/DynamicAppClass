using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Contacts;

namespace DynamicAppClass.Application.Services;

public sealed class ContactService(IClassInstanceContactRepository classInstanceContacts, IContactConfigRepository contactConfig)
{
    public async Task<IReadOnlyList<ClassInstanceContactDto>> GetInstanceContacts(int instanceId, CancellationToken cancellationToken)
        => [.. (await classInstanceContacts.GetAsync(instanceId, cancellationToken)).Select(MapToDto)];

    public IReadOnlyList<AllowedContactDto> GetConfig(int typeId)
        => [.. contactConfig.GetConfig(typeId).Select(MapToAllowedDto)];

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
            State = request.State,
            ZipCode = request.ZipCode, 
            Country = request.Country, 
            LicenseNumber = request.LicenseNumber
        };
        await classInstanceContacts.AddAsync(contact, cancellationToken);
        await classInstanceContacts.SaveChangesAsync(cancellationToken);
        return MapToDto(contact);
    }

    public async Task<AllowedContactDto> CreateConfig(AddConfigRequest request, CancellationToken cancellationToken)
    {
        var contact = new AllowedContact
        {
            ClassTypeId = request.ClassTypeId,
            ContactTypeValue = request.ContactTypeValue,
            ContactTypeCaption = request.ContactTypeCaption,
            CanBeEntity = request.CanBeEntity,
            QuantityAllowed = request.QuantityAllowed,
            QuantityRequired = request.QuantityRequired,
            Required = request.Required,
            RequireAddress = request.RequireAddress,
            RequireEmail = request.RequireEmail,
            RequirePhone = request.RequirePhone,
            RequireLicense = request.RequireLicense
        };
        await contactConfig.AddAsync(contact, cancellationToken);
        await contactConfig.SaveChangesAsync(cancellationToken);
        return MapToAllowedDto(contact);
    }

    public async Task RemoveInstanceContact(int contactId, CancellationToken cancellationToken)
    {
        var contact = await classInstanceContacts.GetByIdAsync(contactId, cancellationToken)
            ?? throw new InvalidOperationException($"Contact with id {contactId} not found.");

        classInstanceContacts.Remove(contact);
        await classInstanceContacts.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveConfigContact(int configId, CancellationToken cancellationToken)
    {
        var config = await contactConfig.GetByIdAsync(configId, cancellationToken)
            ?? throw new InvalidOperationException($"Config with id {configId} not found.");

        contactConfig.Remove(config);
        await contactConfig.SaveChangesAsync(cancellationToken);
    }

    #region Helpers
    private static ClassInstanceContactDto MapToDto(ClassInstanceContact contact)
        => new(contact.Id, contact.FirstName, contact.LastName, contact.ContactType, contact.IsEntity, contact.Email, contact.Phone,
            contact.EntityName, contact.Address1, contact.Address2, contact.City, contact.State, contact.ZipCode, contact.Country, contact.LicenseNumber);
    
    private static AllowedContactDto MapToAllowedDto(AllowedContact contact)
        => new(contact.Id, contact.ContactTypeValue, contact.ContactTypeCaption, contact.Required, contact.QuantityRequired,
            contact.QuantityAllowed, contact.CanBeEntity, contact.RequirePhone, contact.RequireEmail, contact.RequireAddress, contact.RequireLicense);
    #endregion
}