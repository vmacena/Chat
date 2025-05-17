using System.Security.Claims;
using Chat.Business.Exceptions;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;

    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// find a conversation between two users
    /// </summary>
    [HttpGet("conversation/{otherUserId:guid}")]
    public async Task<IActionResult> GetConversation(Guid otherUserId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        try
        {
            var messages = await _messageService.GetConversationIfParticipantAsync(
                userId,
                otherUserId
            );
            return messages != null ? Ok(messages) : Forbid();
        }
        catch (ConversationAccessDeniedException)
        {
            return Forbid();
        }
    }
}
