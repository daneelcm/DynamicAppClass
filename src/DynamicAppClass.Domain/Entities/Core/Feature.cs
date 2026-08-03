namespace DynamicAppClass.Domain.Entities.Core;

public sealed class Feature : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsFieldFeature { get; set; } = false;
}
