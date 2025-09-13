using System.ComponentModel.DataAnnotations;

namespace OrderService.Dto;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public required string CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemResponseDto> OrderItems { get; set; } = [];
}


public class CreateOrderDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Order must contain at least 1 item")]
    public required List<CreateOrderItemDto> Items { get; set; }
}

