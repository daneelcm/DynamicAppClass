namespace DynamicAppClass.Domain.Entities;

public sealed class ClassInstance : BaseEntity
{
    public int ClassTypeId { get; set; }
    public int CurrentStatusId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    public ClassType ClassType { get; set; } = null!;
    public ClassStatus CurrentStatus { get; set; } = null!;
    public List<ClassInstanceFieldValue> FieldValues { get; set; } = [];

    public IEnumerable<ClassAction> GetAvailableActions(ClassType classType)
    {
        EnsureBelongsTo(classType);
        var actions = classType.Actions.Where(action => ((action.ConditionClassFieldId ?? 0) == 0) ||
            (FieldValues.Any(fv => fv.ClassFieldId == action.ConditionClassFieldId && fv.Value == action.ConditionValue)));
        return actions;
    }

    public void Execute(ClassType classType, int actionId)
    {
        EnsureBelongsTo(classType);
        var action = classType.Actions.Single(candidate => candidate.Id == actionId) ??
            throw new InvalidOperationException("The requested action does not exist for this class type.");

        if (action.ConditionClassFieldId > 0)
        {
            var field = FieldValues.Single(fv => fv.ClassFieldId == action.ConditionClassFieldId) ??
                throw new InvalidOperationException("The requested action is not valid for the field of this instance's");

            if (field.Value != action.ConditionValue)
                throw new InvalidOperationException("The requested action is not valid for the instance's current value.");
        }

        var assignField = FieldValues.Single(fv => fv.ClassFieldId == action.AssignClassFieldId) ??
            throw new InvalidOperationException("The requested action is not valid for the field of this instance's");

        assignField.Value = action.ValueToAssign;
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
