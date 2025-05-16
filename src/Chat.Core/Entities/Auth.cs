using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Core.Entities;

[Table("auth")]
public partial class Auth
{
    [Key]
    [Column("userid")]
    public Guid Userid { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("passwordhash")]
    public string Passwordhash { get; set; } = null!;

    [Column("lastlogin", TypeName = "timestamp without time zone")]
    public DateTime? Lastlogin { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Auth")]
    public virtual User User { get; set; } = null!;
}
