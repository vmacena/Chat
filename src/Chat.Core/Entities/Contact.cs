namespace Chat.Core.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
    public Guid ContactId { get; set; }
    public required User ContactUser { get; set; }
}
