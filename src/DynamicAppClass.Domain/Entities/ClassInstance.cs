namespace DynamicAppClass.Domain.Entities;

public sealed class ClassInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClassTypeId { get; set; }
    public Guid CurrentStatusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();
    public List<ClassInstanceFieldValue> FieldValues { get; set; } = [];

    public IEnumerable<ClassAction> GetAvailableActions(ClassType classType)
    {
        EnsureBelongsTo(classType);
        return classType.Actions.Where(action => action.FromStatusId == CurrentStatusId);
    }

    public void Execute(ClassType classType, Guid actionId)
    {
        EnsureBelongsTo(classType);
        var action = classType.Actions.SingleOrDefault(candidate => candidate.Id == actionId);
        if (action is null)
        {
            throw new InvalidOperationException("The requested action does not exist for this class type.");
        }

        if (action.FromStatusId != CurrentStatusId)
        {
            throw new InvalidOperationException("The requested action is not valid for the instance's current status.");
        }

        CurrentStatusId = action.ToStatusId;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureBelongsTo(ClassType classType)
    {
        if (classType.Id != ClassTypeId)
        {
            throw new InvalidOperationException("The instance does not belong to the supplied class type.");
        }
    }
}
