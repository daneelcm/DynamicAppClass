namespace DynamicAppClass.Domain.Entities;

public sealed class ClassType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ConcurrencyToken { get; set; } = Guid.NewGuid();

    public List<ClassField> Fields { get; set; } = [];
    public List<ClassStatus> Statuses { get; set; } = [];
    public List<ClassAction> Actions { get; set; } = [];

    public ClassStatus? InitialStatus => Statuses.OrderBy(status => status.SortOrder).FirstOrDefault();

    public void AddField(ClassField field)
    {
        if (Fields.Any(existing => string.Equals(existing.Label, field.Label, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A field named '{field.Label}' already exists.");
        }

        Fields.Add(field);
    }

    public void AddStatus(ClassStatus status)
    {
        if (Statuses.Any(existing => string.Equals(existing.Name, status.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A status named '{status.Name}' already exists.");
        }

        Statuses.Add(status);
    }

    public void AddAction(ClassAction action)
    {
        if (Statuses.All(status => status.Id != action.FromStatusId) || Statuses.All(status => status.Id != action.ToStatusId))
        {
            throw new InvalidOperationException("Action statuses must belong to the class type.");
        }

        Actions.Add(action);
    }
}
