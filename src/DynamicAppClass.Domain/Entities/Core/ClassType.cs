namespace DynamicAppClass.Domain.Entities.Core;

public sealed class ClassType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    public List<ClassField> Fields { get; set; } = [];
    public List<ClassAction> Actions { get; set; } = [];

    public void AddField(ClassField field)
    {
        if (Fields.Any(existing => string.Equals(existing.Label, field.Label, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A field named '{field.Label}' already exists.");
        }

        Fields.Add(field);
    }

    public void AddAction(ClassAction action)
    {
        if (Fields.All(field => field.Id != action.AssignClassFieldId) || (action.ConditionClassFieldId.HasValue && Fields.All(field => field.Id != action.ConditionClassFieldId.Value)))
        {
            throw new InvalidOperationException("Action field must belong to the class type.");
        }

        Actions.Add(action);
    }
}
