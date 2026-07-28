namespace DynamicAppClass.Domain.Entities.Core;

public sealed class ClassInstanceFieldValue : BaseEntity
{
    public int ClassInstanceId { get; set; }
    public int ClassFieldId { get; set; }
    public string? Value { get; set; }

    public ClassField ClassField { get; set; } = null!;
}
