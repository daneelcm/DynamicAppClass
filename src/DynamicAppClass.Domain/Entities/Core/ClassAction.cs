namespace DynamicAppClass.Domain.Entities.Core;

public sealed class ClassAction : BaseEntity
{
    public int ClassTypeId { get; set; }
    public required string Name { get; set; }
    
    public int AssignClassFieldId { get; set; }
    public required string ValueToAssign { get; set; }
    
    public int? ConditionClassFieldId { get; set; }
    public string? ConditionValue { get; set; }


    public ClassField AssignClassField { get; set; } = null!;
    public ClassField? ConditionClassField { get; set; }
}
