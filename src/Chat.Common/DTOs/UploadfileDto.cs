namespace Chat.Common.DTOs;

public class UploadfileDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string Mimetype { get; set; } = null!;
    public string Url { get; set; } = null!;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}
