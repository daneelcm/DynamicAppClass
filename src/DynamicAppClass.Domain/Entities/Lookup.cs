namespace DynamicAppClass.Domain.Entities;

public sealed class Lookup : BaseEntity
{
    public int FieldId { get; set; }
    public required string Value { get; set; }
    public required string Caption { get; set; }
    public int? SortOrder { get; set; }
}
