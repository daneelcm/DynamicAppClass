namespace DynamicAppClass.Domain.Entities.Core;

public sealed class ClassType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    public List<ClassField> Fields { get; set; } = [];
    public List<ClassAction> Actions { get; set; } = [];
    public List<ClassTypeFeature> Features { get; set; } = [];
}
