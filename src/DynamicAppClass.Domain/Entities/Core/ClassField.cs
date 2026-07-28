namespace DynamicAppClass.Domain.Entities.Core;

public sealed class ClassField : BaseEntity
{
    public int ClassTypeId { get; set; }
    public int FieldId { get; set; }
    public int? DependsOnClassFieldId { get; set; }
    public string? DependsOnClassFieldValue { get; set; }
    public string? OverrideName { get; set; }
    public bool IsRequired { get; set; }
    public bool IsHidden { get; set; }
    public int SortOrder { get; set; }
    public string? DefaultValue { get; set; }

    public string Label => OverrideName ?? Field.Name;
    public Field Field { get; set; } = null!;
    public ClassField? DependsOnClassField { get; set; }
    public List<ClassFieldLookup> Options { get; set; } = [];
}
