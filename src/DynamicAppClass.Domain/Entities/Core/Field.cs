using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Domain.Entities.Core;

public sealed class Field : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ClassFieldType FieldType { get; set; }

    public List<Lookup> Options { get; set; } = [];
}
