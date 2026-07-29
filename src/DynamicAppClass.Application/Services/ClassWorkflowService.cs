using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Interfaces;
using DynamicAppClass.Domain.Entities.Core;
using DynamicAppClass.Domain.Enums;
using Newtonsoft.Json;

namespace DynamicAppClass.Application.Services;

public sealed class ClassWorkflowService(IClassTypeRepository classTypes, IClassInstanceRepository classInstances,
    IClassFieldRepository classFields, IClassActionRepository classActions, IClassInstanceFieldValueRepository classInstanceFieldValues,
    IFeatureRepository features, IClassTypeFeatureRepository classTypeFeatures)
{
    public async Task<IReadOnlyList<ClassTypeSummaryDto>> ListClassTypesAsync(CancellationToken cancellationToken)
    {
        var results = await classTypes.ListAsync(cancellationToken);
        return [.. results.Select(MapSummary)];
    }

    public async Task<ClassTypeDetailDto> GetClassTypeAsync(int id, CancellationToken cancellationToken)
    {
        var classType = await RequireClassType(id, cancellationToken);
        return await MapDetailAsync(classType, cancellationToken);
    }

    public async Task<string?> GetFeatureConfigurationAsync(int typeId, int featureId, CancellationToken cancellationToken)
    {
        var classTypeFeature = await classTypeFeatures.GetAsync(typeId, featureId, cancellationToken) ??
            throw new InvalidOperationException($"FeatureId {featureId} for Class Type {typeId} not enabled yet.");

        return classTypeFeature.ConfigJson;
    }

    public async Task<ClassTypeDetailDto> CreateClassTypeAsync(CreateClassTypeRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Name");
        var classType = new ClassType { Name = request.Name.Trim(), Description = request.Description?.Trim() ?? string.Empty };
        await classTypes.AddAsync(classType, cancellationToken);
        await classTypes.SaveChangesAsync(cancellationToken);
        return await MapDetailAsync(classType, cancellationToken);
    }

    public async Task<ClassTypeDetailDto> AddFieldAsync(int classTypeId, AddFieldRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Field name");
        var classType = await RequireClassType(classTypeId, cancellationToken);
        ValidateConcurrencyToken(classType.ConcurrencyToken, request.ConcurrencyToken);
        var lookups = request.Options?.Select((option, index) => new Lookup { Caption = option.Trim(), Value = index.ToString() }).ToList() ?? [];
        await classFields.AddAsync(new ClassField
        {
            ClassTypeId = classTypeId,
            IsRequired = request.IsRequired,
            IsHidden = request.IsHidden,
            DefaultValue = request.DefaultValue,
            SortOrder = request.SortOrder,
            DependsOnClassFieldId = request.DependsOnClassFieldId,
            DependsOnClassFieldValue = request.DependsOnClassFieldValue,
            Field = new Field
            {
                Name = request.Name.Trim(),
                FieldType = request.FieldType,
                Options = lookups
            },
            Options = lookups?.Select(lookup => new ClassFieldLookup { Lookup = lookup }).ToList() ?? []
        }, cancellationToken);
        await classFields.SaveChangesAsync(cancellationToken);
        return await MapDetailAsync(classType, cancellationToken);
    }

    public async Task<ClassTypeDetailDto> AddActionAsync(int classTypeId, AddActionRequest request, CancellationToken cancellationToken)
    {
        ValidateText(request.Name, "Action name");
        var classType = await RequireClassType(classTypeId, cancellationToken);
        ValidateConcurrencyToken(classType.ConcurrencyToken, request.ConcurrencyToken);
        await classActions.AddAsync(
            new ClassAction {
                ClassTypeId = classTypeId,
                Name = request.Name.Trim(),
                AssignClassFieldId = request.AssignClassFieldId,
                ValueToAssign = request.ValueToAssign.Trim(),
                ConditionClassFieldId = request.ConditionClassFieldId,
                ConditionValue = request.ConditionValue?.Trim()
            }, cancellationToken
        );
        await classActions.SaveChangesAsync(cancellationToken);
        return await MapDetailAsync(classType, cancellationToken);
    }

    public async Task<ClassTypeDetailDto> UpdateFeatureAsync(int classTypeId, ClassTypeFeatureRequest request, CancellationToken cancellationToken)
    {
        var classType = await RequireClassType(classTypeId, cancellationToken);
        var existingFeature = classType.Features.SingleOrDefault(f => f.FeatureId == request.Id && f.ClassTypeId == classTypeId);
        if (existingFeature is not null)
        {
            existingFeature.Active = request.IsEnabled;
            existingFeature.ConfigJson = request.ConfigurationJson;
            classTypeFeatures.Update(existingFeature);
        }
        else
        {
            await classTypeFeatures.AddAsync(
                new ClassTypeFeature
                {
                    ClassTypeId = classTypeId,
                    FeatureId = request.Id,
                    Active = request.IsEnabled
                }, cancellationToken
            );
        }
        await classTypeFeatures.SaveChangesAsync(cancellationToken);
        return await MapDetailAsync(classType, cancellationToken);
    }

    public async Task<IReadOnlyList<ClassInstanceSummaryDto>> ListInstancesAsync(CancellationToken cancellationToken)
    {
        var instances = await classInstances.ListAsync(cancellationToken);
        var types = await classTypes.ListAsync(cancellationToken);
        return instances.Select(instance => MapInstanceSummary(instance, types.Single(type => type.Id == instance.ClassTypeId))).ToList();
    }

    public async Task<ClassInstanceDetailDto> GetInstanceAsync(int id, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(id, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    public async Task<ClassInstanceDetailDto> CreateInstanceAsync(CreateClassInstanceRequest request, CancellationToken cancellationToken)
    {
        var classType = await RequireClassType(request.ClassTypeId, cancellationToken);
        ValidateFieldValues(classType, request.FieldValues);

        var instance = new ClassInstance
        {
            ClassTypeId = classType.Id,
            FieldValues = request.FieldValues.Select(pair => new ClassInstanceFieldValue { ClassFieldId = pair.Key, Value = pair.Value }).ToList()
        };

        await classInstances.AddAsync(instance, cancellationToken);
        await classInstances.SaveChangesAsync(cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    public async Task<ClassInstanceDetailDto> UpdateFieldValuesAsync(int instanceId, UpdateClassInstanceValuesRequest request, CancellationToken cancellationToken)
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

        instance.UpdatedAt = DateTime.UtcNow;
        await classInstances.SaveChangesAsync(cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    public async Task<IReadOnlyList<ClassActionDto>> GetAvailableActionsAsync(int instanceId, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(instanceId, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        return instance.GetAvailableActions(classType).Select(action => MapAction(action, classType)).ToList();
    }

    public async Task<ClassInstanceDetailDto> ExecuteActionAsync(int instanceId, ExecuteActionRequest request, CancellationToken cancellationToken)
    {
        var instance = await RequireInstance(instanceId, cancellationToken);
        var classType = await RequireClassType(instance.ClassTypeId, cancellationToken);
        ValidateConcurrencyToken(instance.ConcurrencyToken, request.ConcurrencyToken);
        instance.Execute(classType, request.ActionId);
        await classInstances.SaveChangesAsync(cancellationToken);
        await classInstanceFieldValues.SaveChangesAsync(cancellationToken);
        return MapInstanceDetail(instance, classType);
    }

    private async Task<ClassType> RequireClassType(int id, CancellationToken cancellationToken) =>
        await classTypes.GetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Class type was not found.");

    private async Task<ClassInstance> RequireInstance(int id, CancellationToken cancellationToken) =>
        await classInstances.GetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Class instance was not found.");

    private static void ValidateConcurrencyToken(Guid current, Guid provided)
    {
        if (current != provided)
        {
            throw new InvalidOperationException("The resource has been modified by another user. Please refresh and try again.");
        }
    }

    private static void ValidateFieldValues(ClassType classType, IReadOnlyDictionary<int, string?> values)
    {
        var fieldIds = classType.Fields.Select(field => field.Id).ToHashSet();
        if (values.Keys.Any(fieldId => !fieldIds.Contains(fieldId)))
        {
            throw new InvalidOperationException("One or more field values do not belong to the class type.");
        }

        foreach (var field in classType.Fields.Where(field => field.IsRequired))
        {
            if (field.DependsOnClassFieldId > 0 && values.TryGetValue(field.DependsOnClassFieldId.Value, out var depVal) && depVal != field.DependsOnClassFieldValue)
            {
                continue;
            }
            if (!values.TryGetValue(field.Id, out var value) || string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Field '{field.Label}' is required.");
            }
        }
    }

    private static string GetTitleFromFields(ClassType classType, IReadOnlyDictionary<int, string?> values)
    {
        var titleField = classType.Fields.FirstOrDefault(field => field.Label.Equals("Title", StringComparison.OrdinalIgnoreCase))
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
        new(classType.Id, classType.Name, classType.Description, classType.Fields.Count, classType.Actions.Count, classType.ConcurrencyToken);

    private async Task<ClassTypeDetailDto> MapDetailAsync(ClassType classType, CancellationToken token)
    {
        var allFeatures = await features.ListAsync(token);
        return new(classType.Id, classType.Name, classType.Description,
            [..classType.Fields.OrderBy(field => field.SortOrder).Select(MapField)],
            [..classType.Actions.Select(action => MapAction(action, classType))],
            [..allFeatures.Select(x => new ClassTypeFeatureDto(x.Id, x.Code, x.Name, 
                classType.Features.FirstOrDefault(f => f.FeatureId == x.Id)?.Active ?? false, 
                classType.Features.FirstOrDefault(f => f.FeatureId == x.Id)?.ConfigJson))
                .OrderByDescending(x => x.IsEnabled).ThenBy(x => x.Name)],
            classType.ConcurrencyToken);
    }

    private static ClassFieldDto MapField(ClassField field) =>
        new(field.Id, field.Label, field.Field.FieldType, field.IsRequired, field.IsHidden, field.DefaultValue, field.SortOrder, field.DependsOnClassFieldId, field.DependsOnClassFieldValue, [..field.Options?.OrderBy(x => x.SortOrder).Select(x => new FieldOptionsDto(x.Value, x.Caption)) ?? []]);

    private static ClassActionDto MapAction(ClassAction action, ClassType classType)
    {
        var assignField = classType.Fields.Single(field => field.Id == action.AssignClassFieldId);
        var conditionField = classType.Fields.SingleOrDefault(field => field.Id == action.ConditionClassFieldId);
        return new(
            action.Id, action.Name, assignField.Id, assignField.Label, 
            assignField.Field.FieldType == ClassFieldType.Select ?
                assignField.Options.First(x => x.Value == action.ValueToAssign).Caption : action.ValueToAssign, 
            conditionField?.Id, conditionField?.Label,
            conditionField?.Field.FieldType == ClassFieldType.Select ?
                conditionField.Options.First(x => x.Value == action.ConditionValue).Caption : action.ConditionValue);
    }

    private static ClassInstanceSummaryDto MapInstanceSummary(ClassInstance instance, ClassType classType)
    {
        var statusField = classType.Fields.First(x => x.Field.Name == "Status");
        var statusVal = instance.FieldValues.First(fv => fv.ClassFieldId == statusField.Id).Value;
        var title = GetTitleFromFields(classType, instance.FieldValues.ToDictionary(fv => fv.ClassFieldId, fv => fv.Value));
        return new(instance.Id, classType.Id, classType.Name, title, statusField.Options.First(x => x.Value == statusVal).Caption, instance.CreatedAt, instance.UpdatedAt, instance.ConcurrencyToken);
    }

    private static ClassInstanceDetailDto MapInstanceDetail(ClassInstance instance, ClassType classType)
    {
        var values = classType.Fields.OrderBy(field => field.SortOrder)
            .Select(field => new ClassInstanceFieldValueDto(field.Id, field.Label, instance.FieldValues.SingleOrDefault(value => value.ClassFieldId == field.Id)?.Value))
            .ToList();
        var title = GetTitleFromFields(classType, instance.FieldValues.ToDictionary(fv => fv.ClassFieldId, fv => fv.Value));
        return new(instance.Id, classType.Id, classType.Name, title, values, instance.GetAvailableActions(classType).Select(action => MapAction(action, classType)).ToList(), instance.CreatedAt, instance.UpdatedAt, instance.ConcurrencyToken);
    }
}
