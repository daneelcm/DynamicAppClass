namespace DynamicAppClass.Domain.Entities;

public sealed class ClassInstanceFieldValue : BaseEntity
{
    public int ClassInstanceId { get; set; }
    public int ClassFieldId { get; set; }
    public string? Value { get; set; }
}
