using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Domain.Entities.Contacts
{
    public sealed class ClassInstanceContact : BaseEntity
    {
        public int ClassInstanceId { get; set; }

        public required string ContactType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsEntity { get; set; }
        public string? EntityName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? LicenseNumber { get; set; }

        public ClassInstance ClassInstance { get; set; } = null!;
    }
}
