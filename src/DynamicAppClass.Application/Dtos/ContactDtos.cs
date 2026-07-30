namespace DynamicAppClass.Application.Dtos;

public sealed record ClassInstanceContactDto(int Id, string? FirstName, string? LastName, string ContactType, bool IsEntity, string? Email,
        string? Phone, string? EntityName, string? Address1, string? Address2, string? City, string? State, string? ZipCode, string? Country, string? LicenseNumber);
public sealed record AddContactRequest(int InstanceId, string? FirstName, string? LastName, string ContactType, bool IsEntity, string? Email,
        string? Phone, string? EntityName, string? Address1, string? Address2, string? City, string? State, string? ZipCode, string? Country, string? LicenseNumber);
public sealed record AllowedContactDto(int Id, string ContactTypeValue, string ContactTypeCaption, bool Required, int QuantityRequired,
        int QuantityAllowed, bool CanBeEntity, bool RequirePhone, bool RequireEmail, bool RequireAddress, bool RequireLicense);

public sealed record AddConfigRequest(int ClassTypeId, string ContactTypeValue, string ContactTypeCaption, bool Required, int QuantityRequired,
        int QuantityAllowed, bool CanBeEntity, bool RequirePhone, bool RequireEmail, bool RequireAddress, bool RequireLicense);

