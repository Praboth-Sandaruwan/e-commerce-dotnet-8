using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Models;

public class OrderItem : BaseEntity
{
    [Required]
    public required string ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public required string ProductName { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    // Foreign key to the Order
    public Guid OrderId { get; set; }
    
    [JsonIgnore] // Prevents circular references during serialization
    public Order? Order { get; set; }
}