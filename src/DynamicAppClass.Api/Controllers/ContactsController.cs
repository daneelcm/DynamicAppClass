using DynamicAppClass.Application.Dtos;
using DynamicAppClass.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicAppClass.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public sealed class ContactsController(ContactService contactService) : ControllerBase
{
    [HttpGet("{instanceId:int}")]
    public async Task<ActionResult<IReadOnlyList<ClassInstanceContactDto>>> Get(int instanceId, CancellationToken cancellationToken)
        => Ok(await contactService.GetInstanceContacts(instanceId, cancellationToken));

    [HttpGet("config/{typeId:int}")]
    public ActionResult<IReadOnlyList<AllowedContactDto>> GetConfig(int typeId)
        => Ok(contactService.GetConfig(typeId));

    [HttpPost]
    public async Task<ActionResult<ClassInstanceContactDto>> Create(AddContactRequest request, CancellationToken cancellationToken)
        => Ok(await contactService.CreateInstanceContact(request, cancellationToken));

    [HttpPost("config")]
    public async Task<ActionResult<ClassInstanceContactDto>> CreateConfig(AddConfigRequest request, CancellationToken cancellationToken)
        => Ok(await contactService.CreateConfig(request, cancellationToken));

    [HttpDelete("{contactId:int}")]
    public async Task<IActionResult> Delete(int contactId, CancellationToken cancellationToken)
    {
        await contactService.RemoveInstanceContact(contactId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("config/{typeId:int}")]
    public async Task<IActionResult> DeleteConfig(int typeId, CancellationToken cancellationToken)
    {
        await contactService.RemoveConfigContact(typeId, cancellationToken);
        return NoContent();
    }
}
