using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Core.Entities;

[Table("users")]
public partial class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("publicid")]
    public string Publicid { get; set; } = null!;

    [Column("passwordhash")]
    public string Passwordhash { get; set; } = null!;

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [InverseProperty("User")]
    public virtual Auth? Auth { get; set; }

    [InverseProperty("ContactNavigation")]
    public virtual ICollection<Contact> ContactContactNavigations { get; set; } = new List<Contact>();

    [InverseProperty("User")]
    public virtual ICollection<Contact> ContactUsers { get; set; } = new List<Contact>();

    [InverseProperty("Receiver")]
    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    [InverseProperty("Sender")]
    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();
}
