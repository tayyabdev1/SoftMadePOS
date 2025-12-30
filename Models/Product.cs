using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Barcode { get; set; } = "";
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(50)]
    public string Category { get; set; } = "General";

    [Column(TypeName = "decimal(18, 2)")]
    public decimal CostPrice { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SalePrice { get; set; }

    public decimal StockQuantity { get; set; }

    public int MinStockAlert { get; set; } = 10; //Low Stock alert

    public DateTime? ExpireyDate{ get; set; } //Null if not applicable

    public bool IsActive { get; set; } = true;

}

