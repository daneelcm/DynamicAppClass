namespace DynamicAppClass.Domain.Entities;

public sealed class Lookup : BaseEntity
{
    public int FieldId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
}
