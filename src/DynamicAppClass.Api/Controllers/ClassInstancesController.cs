using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicAppClass.Api.Controllers;

[ApiController]
[Route("api/class-instances")]
public sealed class ClassInstancesController(ClassWorkflowService workflowService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClassInstanceSummaryDto>>> List(CancellationToken cancellationToken) =>
        Ok(await workflowService.ListInstancesAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClassInstanceDetailDto>> Get(int id, CancellationToken cancellationToken) =>
        Ok(await workflowService.GetInstanceAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ClassInstanceDetailDto>> Create(CreateClassInstanceRequest request, CancellationToken cancellationToken)
    {
        var created = await workflowService.CreateInstanceAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/field-values")]
    public async Task<ActionResult<ClassInstanceDetailDto>> UpdateFieldValues(int id, UpdateClassInstanceValuesRequest request, CancellationToken cancellationToken) =>
        Ok(await workflowService.UpdateFieldValuesAsync(id, request, cancellationToken));

    [HttpGet("{id:int}/available-actions")]
    public async Task<ActionResult<IReadOnlyList<ClassActionDto>>> GetAvailableActions(int id, CancellationToken cancellationToken) =>
        Ok(await workflowService.GetAvailableActionsAsync(id, cancellationToken));

    [HttpPost("{id:int}/actions")]
    public async Task<ActionResult<ClassInstanceDetailDto>> ExecuteAction(int id, ExecuteActionRequest request, CancellationToken cancellationToken) =>
        Ok(await workflowService.ExecuteActionAsync(id, request, cancellationToken));
}
