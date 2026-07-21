namespace DynamicAppClass.Domain.Entities;

public sealed record WorkflowTransition(int ActionId, int FromStatusId, int ToStatusId);
