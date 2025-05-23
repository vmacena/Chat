using System.Security.Claims;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Chat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[Authorize]
[ApiController]
[Route("api/contacts")]
public class ContactController : ControllerBase
{
    private readonly ContactService _contactService;

    public ContactController(ContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var result = await _contactService.GetContactsWithLastMessageAsync(userId);
        return Ok(result);
    }

    [HttpPost("by-email")]
    public async Task<IActionResult> AddContactByEmail([FromBody] AddContactByEmailDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        try
        {
            await _contactService.AddContactByEmailAsync(userId, dto.Email);
            return Ok(new { message = "Contato adicionado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
