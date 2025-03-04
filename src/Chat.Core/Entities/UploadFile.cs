namespace Chat.Core.Entities;

public class UploadFile
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Message? Message { get; set; }
    public string? Type { get; set; }
    public string? Url { get; set; }
    public long Size { get; set; }
}
