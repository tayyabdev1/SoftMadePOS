using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shop.Models;
public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = "";

    [MaxLength(20)]
    public string Phone { get; set; } = "General";

    [MaxLength(200)]
    public string Address { get; set; } = "";

    // Positive = They owe us, Negative = We owe them
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; } = 0;

}

