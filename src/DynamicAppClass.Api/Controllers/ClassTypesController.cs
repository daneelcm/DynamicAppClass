using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicAppClass.Api.Controllers;

[ApiController]
[Route("api/class-types")]
public sealed class ClassTypesController(ClassWorkflowService workflowService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassTypeSummaryDto>>> List(CancellationToken cancellationToken) =>
        Ok(await workflowService.ListClassTypesAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClassTypeDetailDto>> Get(Guid id, CancellationToken cancellationToken) =>
        Ok(await workflowService.GetClassTypeAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ClassTypeDetailDto>> Create(CreateClassTypeRequest request, CancellationToken cancellationToken)
    {
        var created = await workflowService.CreateClassTypeAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("{id:guid}/fields")]
    public async Task<ActionResult<ClassTypeDetailDto>> AddField(Guid id, AddFieldRequest request, CancellationToken cancellationToken) =>
        Ok(await workflowService.AddFieldAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/statuses")]
    public async Task<ActionResult<ClassTypeDetailDto>> AddStatus(Guid id, AddStatusRequest request, CancellationToken cancellationToken) =>
        Ok(await workflowService.AddStatusAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/actions")]
    public async Task<ActionResult<ClassTypeDetailDto>> AddAction(Guid id, AddActionRequest request, CancellationToken cancellationToken) =>
        Ok(await workflowService.AddActionAsync(id, request, cancellationToken));
}
