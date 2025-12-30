using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
namespace Shop.Models;
public class Sale
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
    public int? CustomerId { get; set; } // Null if walk-in customer

    public Customer? Customer { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal NetTotal { get; set; } // Final paid comment

    public string PaymentMethod { get; set; }

    public List<SaleDetails> SaleDetails { get; set; } = new();

}

public class SaleDetails
{
    [Key]
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale? Sale { get; set; }
    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public decimal Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SalePrice { get; set; } // Price at the moment of Sale
}

