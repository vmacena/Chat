namespace Chat.Core.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public required User Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public required User Receiver { get; set; }
    public string? Content { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
    public ICollection<UploadFile>? UploadFiles { get; set; }
}
