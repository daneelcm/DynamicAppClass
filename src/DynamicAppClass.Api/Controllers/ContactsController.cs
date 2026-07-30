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

    [HttpPost]
    public async Task<ActionResult<ClassInstanceContactDto>> Create(AddContactRequest request, CancellationToken cancellationToken)
        => Ok(await contactService.CreateInstanceContact(request, cancellationToken));

    [HttpDelete("{contactId:int}")]
    public async Task<IActionResult> Delete(int contactId, CancellationToken cancellationToken)
    {
        await contactService.RemoveInstanceContact(contactId, cancellationToken);
        return NoContent();
    }
}
