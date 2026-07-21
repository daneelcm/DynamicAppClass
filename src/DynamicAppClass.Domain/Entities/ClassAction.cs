namespace DynamicAppClass.Domain.Entities;

public sealed class ClassAction : BaseEntity
{
    public int ClassTypeId { get; set; }
    public required string Name { get; set; }
    public int FromStatusId { get; set; }
    public int ToStatusId { get; set; }

    public ClassStatus FromStatus { get; set; }
    public ClassStatus ToStatus { get; set; }
    public WorkflowTransition Transition => new(Id, FromStatusId, ToStatusId);
}
