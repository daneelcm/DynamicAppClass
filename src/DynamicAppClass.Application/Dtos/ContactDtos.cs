namespace DynamicAppClass.Application.Dtos;

public sealed record ClassInstanceContactDto(int Id, string? FirstName, string? LastName, string ContactType, bool IsEntity, string? Email,
        string? Phone, string? EntityName, string? Address1, string? Address2, string? City, string? ZipCode, string? Country, string? LicenseNumber);
public sealed record AddContactRequest(int InstanceId, string? FirstName, string? LastName, string ContactType, bool IsEntity, string? Email,
        string? Phone, string? EntityName, string? Address1, string? Address2, string? City, string? ZipCode, string? Country, string? LicenseNumber);
