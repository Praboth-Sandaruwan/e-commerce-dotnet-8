using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Order : BaseEntity
{
    [Required]
    public required string CustomerId { get; set; }

    [Required]
    [MaxLength(200)]
    public required string CustomerName { get; set; }

    [Required]
    public List<OrderItem> OrderItems { get; set; } = new();

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; } = false;

    // This is now nullable, as an order is not paid when it's first created.
    public DateTime? PaidAt { get; set; }
}
