namespace DynamicAppClass.Domain.Entities;

public sealed class ClassInstanceFieldValue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassInstanceId { get; set; }
    public Guid ClassFieldId { get; set; }
    public string? Value { get; set; }
}
