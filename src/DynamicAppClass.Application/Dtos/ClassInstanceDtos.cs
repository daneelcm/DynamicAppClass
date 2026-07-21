namespace DynamicAppClass.Application.Dtos;

public sealed record ClassInstanceSummaryDto(int Id, int ClassTypeId, string ClassTypeName, string Title, int CurrentStatusId, string CurrentStatusName, DateTime CreatedAt, DateTime UpdatedAt, Guid ConcurrencyToken);
public sealed record ClassInstanceDetailDto(int Id, int ClassTypeId, string ClassTypeName, string Title, ClassStatusDto CurrentStatus, IReadOnlyList<ClassInstanceFieldValueDto> FieldValues, IReadOnlyList<ClassActionDto> AvailableActions, DateTime CreatedAt, DateTime UpdatedAt, Guid ConcurrencyToken);
public sealed record ClassInstanceFieldValueDto(int FieldId, string FieldName, string? Value);
public sealed record CreateClassInstanceRequest(int ClassTypeId, string? Title, IReadOnlyDictionary<int, string?> FieldValues);
public sealed record UpdateClassInstanceValuesRequest(IReadOnlyDictionary<int, string?> FieldValues, Guid ConcurrencyToken);
public sealed record ExecuteActionRequest(int ActionId, Guid ConcurrencyToken);
