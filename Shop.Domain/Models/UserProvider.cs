using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("users_providers")]
public class UserProvider
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("provider_id")]
    public int ProviderId { get; set; }

    [Required]
    [Column("number_provider")]
    [MaxLength(255)]
    public string NumberProvider { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
}
