using DynamicAppClass.Domain.Enums;

namespace DynamicAppClass.Application.Dtos;

public sealed record ClassTypeSummaryDto(int Id, string Name, string Description, int FieldCount, int StatusCount, int ActionCount, Guid ConcurrencyToken);
public sealed record ClassTypeDetailDto(int Id, string Name, string Description, IReadOnlyList<ClassFieldDto> Fields, IReadOnlyList<ClassStatusDto> Statuses, IReadOnlyList<ClassActionDto> Actions, Guid ConcurrencyToken);
public sealed record ClassFieldDto(int Id, string Name, ClassFieldType FieldType, bool IsRequired, int SortOrder, IReadOnlyList<FieldOptionsDto> Options);
public sealed record ClassStatusDto(int Id, string Name, int SortOrder);
public sealed record FieldOptionsDto(string value, string Caption);
public sealed record ClassActionDto(int Id, string Name, int FromStatusId, string FromStatusName, int ToStatusId, string ToStatusName);
public sealed record CreateClassTypeRequest(string Name, string Description);
public sealed record AddFieldRequest(string Name, ClassFieldType FieldType, bool IsRequired, int SortOrder, IReadOnlyList<string>? Options, Guid ConcurrencyToken);
public sealed record AddStatusRequest(string Name, int SortOrder, Guid ConcurrencyToken);
public sealed record AddActionRequest(string Name, int FromStatusId, int ToStatusId, Guid ConcurrencyToken);
