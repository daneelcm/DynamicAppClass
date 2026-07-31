using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Domain.Entities.Contacts
{
    public sealed class AllowedContact : BaseEntity
    {
        public int ClassTypeId { get; set; }

        public required string ContactTypeValue { get; set; }
        public required string ContactTypeCaption { get; set; }
        public bool Required { get; set; } = false;
        public int QuantityRequired { get; set; } = 1;
        public int QuantityAllowed { get; set; } = 0;
        public bool CanBeEntity { get; set; } = false;
        public bool RequirePhone { get; set; } = false;
        public bool RequireEmail { get; set; } = false;
        public bool RequireAddress { get; set; } = false;
        public bool RequireLicense { get; set; } = false;

        public ClassType ClassType { get; set; } = null!;
    }
}
