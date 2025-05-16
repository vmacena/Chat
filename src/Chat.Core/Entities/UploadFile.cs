using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chat.Core.Entities;

[Table("uploadfiles")]
[Index("Messageid", Name = "IX_UploadFiles_MessageId")]
public partial class Uploadfile
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("messageid")]
    public Guid Messageid { get; set; }

    [Column("mimetype")]
    public string? Mimetype { get; set; }

    [Column("url")]
    public string? Url { get; set; }

    [Column("size")]
    public long Size { get; set; }

    [Column("uploadedat", TypeName = "timestamp without time zone")]
    public DateTime Uploadedat { get; set; }

    [ForeignKey("Messageid")]
    [InverseProperty("Uploadfiles")]
    public virtual Message Message { get; set; } = null!;
}
