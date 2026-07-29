using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Application.Dtos;

public sealed record ClassTypeSummaryDto(int Id, string Name, string Description, int FieldCount, int ActionCount, Guid ConcurrencyToken);
public sealed record ClassTypeDetailDto(int Id, string Name, string Description, IReadOnlyList<ClassFieldDto> Fields, IReadOnlyList<ClassActionDto> Actions, IReadOnlyList<ClassTypeFeatureDto> Features, Guid ConcurrencyToken);
public sealed record ClassFieldDto(int Id, string Name, ClassFieldType FieldType, bool IsRequired, bool IsHidden, string? DefaultValue, int SortOrder, int? DependsOnClassFieldId, string? DependsOnClassFieldValue, IReadOnlyList<FieldOptionsDto> Options);
public sealed record ClassTypeFeatureDto(int Id, string Code, string Name, bool IsEnabled, string? ConfigurationJson);
public sealed record FieldOptionsDto(string Value, string Caption);
public sealed record ClassActionDto(int Id, string Name, int AssignClassFieldId, string AssignFieldName, string ValueToAssign, int? ConditionClassFieldId, string? ConditionFieldName, string? ConditionValue);
public sealed record CreateClassTypeRequest(string Name, string Description);
public sealed record AddFieldRequest(string Name, ClassFieldType FieldType, bool IsRequired, bool IsHidden, string? DefaultValue, int SortOrder, int? DependsOnClassFieldId, string? DependsOnClassFieldValue, IReadOnlyList<string>? Options, Guid ConcurrencyToken);
public sealed record AddActionRequest(string Name, int AssignClassFieldId, string ValueToAssign, int? ConditionClassFieldId, string? ConditionValue, Guid ConcurrencyToken);
public sealed record ClassTypeFeatureRequest(int Id, bool IsEnabled, string? ConfigurationJson = null);
