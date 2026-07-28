using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Domain.Entities.Core;

public class BaseEntity
{
    public int Id { get; set; }
    public bool Active { get; set; } = true;
}
