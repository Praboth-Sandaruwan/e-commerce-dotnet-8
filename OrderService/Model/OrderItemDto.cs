using System.ComponentModel.DataAnnotations;

namespace OrderService.Dto;


public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public required string ProductId { get; set; }
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public required decimal Subtotal { get; set; }
}


public class CreateOrderItemDto
{
    [Required]
    public required string ProductId { get; set; }

    [Required]
    [MaxLength(200, ErrorMessage = "ProductName cannot exceed 200 characters")]
    public required string ProductName { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be at least 1 and at most 100")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than 0")]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Subtotal must be greater than 0")]
    public decimal Subtotal { get; set; }
}