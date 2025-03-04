namespace Chat.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string IdShare { get; set; }
    public required string Password { get; set; }
    public ICollection<Contact>? Contacts { get; set; }
    public ICollection<Message>? SentMessages { get; set; }
    public ICollection<Message>? ReceivedMessages { get; set; }
}
