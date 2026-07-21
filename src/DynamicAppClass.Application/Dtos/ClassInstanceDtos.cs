namespace DynamicAppClass.Application.Dtos;

public sealed record ClassInstanceSummaryDto(Guid Id, Guid ClassTypeId, string ClassTypeName, string Title, Guid CurrentStatusId, string CurrentStatusName, DateTime CreatedAt, DateTime UpdatedAt, Guid ConcurrencyToken);
public sealed record ClassInstanceDetailDto(Guid Id, Guid ClassTypeId, string ClassTypeName, string Title, ClassStatusDto CurrentStatus, IReadOnlyList<ClassInstanceFieldValueDto> FieldValues, IReadOnlyList<ClassActionDto> AvailableActions, DateTime CreatedAt, DateTime UpdatedAt, Guid ConcurrencyToken);
public sealed record ClassInstanceFieldValueDto(Guid FieldId, string FieldName, string? Value);
public sealed record CreateClassInstanceRequest(Guid ClassTypeId, string? Title, IReadOnlyDictionary<Guid, string?> FieldValues);
public sealed record UpdateClassInstanceValuesRequest(IReadOnlyDictionary<Guid, string?> FieldValues, Guid ConcurrencyToken);
public sealed record ExecuteActionRequest(Guid ActionId, Guid ConcurrencyToken);
