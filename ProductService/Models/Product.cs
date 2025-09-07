using System.ComponentModel.DataAnnotations;

namespace ProductService.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 1000000)]
    public required decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public int StockQuantity { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}