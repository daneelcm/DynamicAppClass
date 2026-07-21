using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities;

namespace DynamicAppClass.Application.Services;

public sealed class ClassWorkflowService(IClassTypeRepository classTypes, IClassInstanceRepository classInstances)
{
    public async Task<IReadOnlyList<ClassTypeSummaryDto>> ListClassTypesAsync(CancellationToken cancellationToken)
    {
        var results = await classTypes.ListAsync(cancellationToken);
        return results.Select(MapSummary).ToList();
    }

    public async Task<ClassTypeDetailDto> GetClassTypeAsync(Guid id, CancellationToken cancellationToken)
    {
        var classType = await RequireClassType(id, cancellationToken);
        return MapDetail(classType);
    }

    public async Task<ClassTypeDetailDto> CreateClassTypeAsync(CreateClassTypeRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Name");
        var classType = new ClassType { Name = request.Name.Trim(), Description = request.Description?.Trim() ?? string.Empty };
        await classTypes.AddAsync(classType, cancellationToken);
        await classTypes.SaveChangesAsync(cancellationToken);
        return MapDetail(classType);
    }

    public async Task<ClassTypeDetailDto> AddFieldAsync(Guid classTypeId, AddFieldRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Field name");
        var classType = await RequireClassType(classTypeId, cancellationToken);
        ValidateConcurrencyToken(classType.ConcurrencyToken, request.ConcurrencyToken);
        classType.AddField(new ClassField
        {
            ClassTypeId = classTypeId,
            Name = request.Name.Trim(),
            FieldType = request.FieldType,
            IsRequired = request.IsRequired,
            SortOrder = request.SortOrder,
            OptionsCsv = request.Options is { Count: > 0 } ? string.Join("|", request.Options.Select(option => option.Trim()).Where(option => option.Length > 0)) : null
        });
        await classTypes.SaveChangesAsync(cancellationToken);
        return MapDetail(classType);
    }

    public async Task<ClassTypeDetailDto> AddStatusAsync(Guid classTypeId, AddStatusRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Status name");
        var classType = await RequireClassType(classTypeId, cancellationToken);
        ValidateConcurrencyToken(classType.ConcurrencyToken, request.ConcurrencyToken);
        classType.AddStatus(new ClassStatus { ClassTypeId = classTypeId, Name = request.Name.Trim(), SortOrder = request.SortOrder });
        await classTypes.SaveChangesAsync(cancellationToken);
        return MapDetail(classType);
    }

    public async Task<ClassTypeDetailDto> AddActionAsync(Guid classTypeId, AddActionRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Action name");
        var classType = await RequireClassType(classTypeId, cancellationToken);
        ValidateConcurrencyToken(classType.ConcurrencyToken, request.ConcurrencyToken);
        classType.AddAction(new ClassAction { ClassTypeId = classTypeId, Name = request.Name.Trim(), FromStatusId = request.FromStatusId, ToStatusId = request.ToStatusId });
        await classTypes.SaveChangesAsync(cancellationToken);
        return MapDetail(classType);
    }

    public async Task<IReadOnlyList<ClassInstanceSummaryDto>> ListInstancesAsync(CancellationToken cancellationToken)
    {
        var instances = await classInstances.ListAsync(cancellationToken);
        var types = await classTypes.ListAsync(cancellationToken);
        return instances.Select(instance => MapInstanceSummary(instance, types.Single(type => type.Id == instance.ClassTypeId))).ToList();
    }

    public async Task<ClassInstanceDetailDto> GetInstanceAsync(Guid id, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(id, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    public async Task<ClassInstanceDetailDto> CreateInstanceAsync(CreateClassInstanceRequest request, CancellationToken cancellationToken)
    {
        var classType = await RequireClassType(request.ClassTypeId, cancellationToken);
        var initialStatus = classType.InitialStatus ?? throw new InvalidOperationException("Class type must have at least one status before instances can be created.");
        ValidateFieldValues(classType, request.FieldValues);

        var title = string.IsNullOrWhiteSpace(request.Title)
            ? GetTitleFromFields(classType, request.FieldValues)
            : request.Title.Trim();

        var instance = new ClassInstance
        {
            ClassTypeId = classType.Id,
            CurrentStatusId = initialStatus.Id,
            Title = title,
            FieldValues = request.FieldValues.Select(pair => new ClassInstanceFieldValue { ClassFieldId = pair.Key, Value = pair.Value }).ToList()
        };

        await classInstances.AddAsync(instance, cancellationToken);
        await classInstances.SaveChangesAsync(cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    public async Task<ClassInstanceDetailDto> UpdateFieldValuesAsync(Guid instanceId, UpdateClassInstanceValuesRequest request, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(instanceId, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        ValidateConcurrencyToken(instance.ConcurrencyToken, request.ConcurrencyToken);
        ValidateFieldValues(classType, request.FieldValues);

        foreach (var field in classType.Fields)
        {
            var existing = instance.FieldValues.SingleOrDefault(value => value.ClassFieldId == field.Id);
            request.FieldValues.TryGetValue(field.Id, out var newValue);
            if (existing is null)
            {
                instance.FieldValues.Add(new ClassInstanceFieldValue { ClassInstanceId = instance.Id, ClassFieldId = field.Id, Value = newValue });
            }
            else
            {
                existing.Value = newValue;
            }
        }

        instance.Title = GetTitleFromFields(classType, request.FieldValues);
        instance.UpdatedAt = DateTime.UtcNow;
        await classInstances.SaveChangesAsync(cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    public async Task<IReadOnlyList<ClassActionDto>> GetAvailableActionsAsync(Guid instanceId, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(instanceId, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        return instance.GetAvailableActions(classType).Select(action => MapAction(action, classType)).ToList();
    }

    public async Task<ClassInstanceDetailDto> ExecuteActionAsync(Guid instanceId, ExecuteActionRequest request, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(instanceId, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        ValidateConcurrencyToken(instance.ConcurrencyToken, request.ConcurrencyToken);
        instance.Execute(classType, request.ActionId);
        await classInstances.SaveChangesAsync(cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    private async Task<ClassType> RequireClassType(Guid id, CancellationToken cancellationToken) =>
        await classTypes.GetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Class type was not found.");

    private async Task<ClassInstance> RequireInstance(Guid id, CancellationToken cancellationToken) =>
        await classInstances.GetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Class instance was not found.");

    private static void ValidateConcurrencyToken(Guid current, Guid provided)
    {
        if (current != provided)
        {
            throw new InvalidOperationException("The resource has been modified by another user. Please refresh and try again.");
        }
    }

    private static void ValidateFieldValues(ClassType classType, IReadOnlyDictionary<Guid, string?> values)
    {
        var fieldIds = classType.Fields.Select(field => field.Id).ToHashSet();
        if (values.Keys.Any(fieldId => !fieldIds.Contains(fieldId)))
        {
            throw new InvalidOperationException("One or more field values do not belong to the class type.");
        }

        foreach (var field in classType.Fields.Where(field => field.IsRequired))
        {
            if (!values.TryGetValue(field.Id, out var value) || string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Field '{field.Name}' is required.");
            }
        }
    }

    private static string GetTitleFromFields(ClassType classType, IReadOnlyDictionary<Guid, string?> values)
    {
        var titleField = classType.Fields.FirstOrDefault(field => field.Name.Equals("Title", StringComparison.OrdinalIgnoreCase))
            ?? classType.Fields.OrderBy(field => field.SortOrder).FirstOrDefault();

        if (titleField is not null && values.TryGetValue(titleField.Id, out var title) && !string.IsNullOrWhiteSpace(title))
        {
            return title.Trim();
        }

        return $"New {classType.Name}";
    }

    private static void ValidateText(string? value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{label} is required.");
        }
    }

    private static ClassTypeSummaryDto MapSummary(ClassType classType) =>
        new(classType.Id, classType.Name, classType.Description, classType.Fields.Count, classType.Statuses.Count, classType.Actions.Count, classType.ConcurrencyToken);

    private static ClassTypeDetailDto MapDetail(ClassType classType) =>
        new(classType.Id, classType.Name, classType.Description, classType.Fields.OrderBy(field => field.SortOrder).Select(MapField).ToList(), classType.Statuses.OrderBy(status => status.SortOrder).Select(MapStatus).ToList(), classType.Actions.Select(action => MapAction(action, classType)).ToList(), classType.ConcurrencyToken);

    private static ClassFieldDto MapField(ClassField field) =>
        new(field.Id, field.Name, field.FieldType, field.IsRequired, field.SortOrder, SplitOptions(field.OptionsCsv));

    private static ClassStatusDto MapStatus(ClassStatus status) =>
        new(status.Id, status.Name, status.SortOrder);

    private static ClassActionDto MapAction(ClassAction action, ClassType classType)
    {
        var from = classType.Statuses.Single(status => status.Id == action.FromStatusId);
        var to = classType.Statuses.Single(status => status.Id == action.ToStatusId);
        return new(action.Id, action.Name, from.Id, from.Name, to.Id, to.Name);
    }

    private static ClassInstanceSummaryDto MapInstanceSummary(ClassInstance instance, ClassType classType)
    {
        var status = classType.Statuses.Single(candidate => candidate.Id == instance.CurrentStatusId);
        return new(instance.Id, classType.Id, classType.Name, instance.Title, status.Id, status.Name, instance.CreatedAt, instance.UpdatedAt, instance.ConcurrencyToken);
    }

    private static ClassInstanceDetailDto MapInstanceDetail(ClassInstance instance, ClassType classType)
    {
        var status = classType.Statuses.Single(candidate => candidate.Id == instance.CurrentStatusId);
        var values = classType.Fields.OrderBy(field => field.SortOrder)
            .Select(field => new ClassInstanceFieldValueDto(field.Id, field.Name, instance.FieldValues.SingleOrDefault(value => value.ClassFieldId == field.Id)?.Value))
            .ToList();

        return new(instance.Id, classType.Id, classType.Name, instance.Title, MapStatus(status), values, instance.GetAvailableActions(classType).Select(action => MapAction(action, classType)).ToList(), instance.CreatedAt, instance.UpdatedAt, instance.ConcurrencyToken);
    }

    private static IReadOnlyList<string> SplitOptions(string? optionsCsv) =>
        string.IsNullOrWhiteSpace(optionsCsv) ? [] : optionsCsv.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
