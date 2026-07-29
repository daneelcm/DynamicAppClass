namespace DynamicAppClass.Domain.Entities.Core;

public sealed class ClassTypeFeature : BaseEntity
{
    public int FeatureId { get; set; }
    public int ClassTypeId { get; set; }
    public string? ConfigJson { get; set; }

    public Feature Feature { get; set; } = null!;
}
