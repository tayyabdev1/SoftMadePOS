using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Shop.Models;
public class Supplier
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string Phone { get; set; } = "";

}

