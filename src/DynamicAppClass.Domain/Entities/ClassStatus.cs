namespace DynamicAppClass.Domain.Entities;

public sealed class ClassStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
