using Microsoft.AspNetCore.Http;

namespace Chat.Common.DTOs;

public class FileUploadRequestDto
{
    public IFormFile File { get; set; } = null!;
    public Guid ReceiverId { get; set; }
}
