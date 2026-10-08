using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models;

[Table("providers")]
public class Provider
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<UserProvider> UserProviders { get; set; } = new List<UserProvider>();
}
