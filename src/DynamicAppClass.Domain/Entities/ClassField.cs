using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Domain.Entities;

public sealed class ClassField : BaseEntity
{
    public int ClassTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ClassFieldType FieldType { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? OptionsCsv { get; set; }
}
