namespace Chat.Common.DTOs;

public class SendMessageRequest
{
    public Guid ReceiverId { get; set; }
    public string? Content { get; set; }
}
