using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class CustomerPayment
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; } // How much they paid
        public string Note { get; set; } = ""; // e.g., "Paid by JazzCash"
    }
}