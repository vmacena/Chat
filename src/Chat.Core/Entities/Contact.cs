using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chat.Core.Entities;

[PrimaryKey("Userid", "Contactid")]
[Table("contacts")]
[Index("Contactid", Name = "IX_Contacts_ContactId")]
[Index("Userid", Name = "IX_Contacts_UserId")]
public partial class Contact
{
    [Column("id")]
    public Guid Id { get; set; }

    [Key]
    [Column("userid")]
    public Guid Userid { get; set; }

    [Key]
    [Column("contactid")]
    public Guid Contactid { get; set; }

    [Column("status")]
    public string Status { get; set; } = null!;

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Contactid")]
    [InverseProperty("ContactContactNavigations")]
    public virtual User ContactNavigation { get; set; } = null!;

    [ForeignKey("Userid")]
    [InverseProperty("ContactUsers")]
    public virtual User User { get; set; } = null!;
}
