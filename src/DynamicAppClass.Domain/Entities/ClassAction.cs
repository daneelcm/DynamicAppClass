namespace DynamicAppClass.Domain.Entities;

public sealed class ClassAction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid FromStatusId { get; set; }
    public Guid ToStatusId { get; set; }
    public WorkflowTransition Transition => new(Id, FromStatusId, ToStatusId);
}
