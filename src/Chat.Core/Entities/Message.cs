using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chat.Core.Entities;

[Table("messages")]
[Index("Receiverid", Name = "IX_Messages_ReceiverId")]
[Index("Senderid", Name = "IX_Messages_SenderId")]
public partial class Message
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("senderid")]
    public Guid Senderid { get; set; }

    [Column("receiverid")]
    public Guid Receiverid { get; set; }

    [Column("content")]
    public string? Content { get; set; }

    [Column("sentat")]
    public DateTime Sentat { get; set; }

    [Column("seenat", TypeName = "timestamp without time zone")]
    public DateTime? Seenat { get; set; }

    [ForeignKey("Receiverid")]
    [InverseProperty("MessageReceivers")]
    public virtual User Receiver { get; set; } = null!;

    [ForeignKey("Senderid")]
    [InverseProperty("MessageSenders")]
    public virtual User Sender { get; set; } = null!;

    [InverseProperty("Message")]
    public virtual ICollection<Uploadfile> Uploadfiles { get; set; } = new List<Uploadfile>();
}
