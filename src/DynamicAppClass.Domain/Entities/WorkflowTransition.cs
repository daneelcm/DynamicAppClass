namespace DynamicAppClass.Domain.Entities;

public sealed record WorkflowTransition(Guid ActionId, Guid FromStatusId, Guid ToStatusId);
