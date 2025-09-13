using OrderService.Dto;
using OrderService.Models;

namespace OrderService.Dto;

public static class OrderResponseMapper
{
    public static OrderResponseDto MapToOrderResponseDto(this Order order)
    {
        try
        {

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount,
                IsPaid = order.IsPaid,
                PaidAt = order.PaidAt,
                OrderDate = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Subtotal
                }).ToList()
            };

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error mapping Order to OrderResponseDto", ex);
        }
    }
}