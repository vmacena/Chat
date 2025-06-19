using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Chat.Business.Services;
using Chat.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UploadController(UploadfileService fileService) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] FileUploadRequestDto request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("No file uploaded.");

        var userIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var senderId))
            return Unauthorized("User not authenticated");

        var dto = await fileService.SaveAsync(request.File, senderId, request.ReceiverId);
        return Ok(dto);
    }

    [HttpGet("{messageId:guid}")]
    public async Task<IActionResult> GetFiles(Guid messageId)
    {
        var list = await fileService.GetByMessageAsync(messageId);
        return Ok(list);
    }
}
