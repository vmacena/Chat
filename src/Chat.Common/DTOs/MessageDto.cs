namespace Chat.Common.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string? Content { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? SeenAt { get; set; }
}
