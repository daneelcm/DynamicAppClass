using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Domain.Entities;

public sealed class ClassField : BaseEntity
{
    public int ClassTypeId { get; set; }
    public int FieldId { get; set; }
    public string? OverrideName { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }

    public string Label => OverrideName ?? Field.Name;
    public Field Field { get; set; } = null!;
}
