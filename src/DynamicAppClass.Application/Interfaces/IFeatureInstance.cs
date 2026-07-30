using DynamicAppClass.Domain.Entities.Core;

namespace DynamicAppClass.Application.Interfaces;

public interface IFeatureInstance
{
    public int ClassInstanceId { get; set; }
    public string? FeatureName { get; set; }

    public ClassInstance ClassInstance { get; set; }
}
