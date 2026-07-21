using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Application.Dtos;

public sealed record ClassTypeSummaryDto(Guid Id, string Name, string Description, int FieldCount, int StatusCount, int ActionCount, Guid ConcurrencyToken);
public sealed record ClassTypeDetailDto(Guid Id, string Name, string Description, IReadOnlyList<ClassFieldDto> Fields, IReadOnlyList<ClassStatusDto> Statuses, IReadOnlyList<ClassActionDto> Actions, Guid ConcurrencyToken);
public sealed record ClassFieldDto(Guid Id, string Name, ClassFieldType FieldType, bool IsRequired, int SortOrder, IReadOnlyList<string> Options);
public sealed record ClassStatusDto(Guid Id, string Name, int SortOrder);
public sealed record ClassActionDto(Guid Id, string Name, Guid FromStatusId, string FromStatusName, Guid ToStatusId, string ToStatusName);
public sealed record CreateClassTypeRequest(string Name, string Description);
public sealed record AddFieldRequest(string Name, ClassFieldType FieldType, bool IsRequired, int SortOrder, IReadOnlyList<string>? Options, Guid ConcurrencyToken);
public sealed record AddStatusRequest(string Name, int SortOrder, Guid ConcurrencyToken);
public sealed record AddActionRequest(string Name, Guid FromStatusId, Guid ToStatusId, Guid ConcurrencyToken);
