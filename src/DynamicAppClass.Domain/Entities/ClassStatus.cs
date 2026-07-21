namespace DynamicAppClass.Domain.Entities;

public sealed class ClassStatus : BaseEntity
{
    public int ClassTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
