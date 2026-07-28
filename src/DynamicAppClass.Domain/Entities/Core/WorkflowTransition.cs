namespace DynamicAppClass.Domain.Entities.Core;

public sealed record WorkflowTransition(int ActionId, int FromStatusId, int ToStatusId);
