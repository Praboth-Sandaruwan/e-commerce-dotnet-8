using System.ComponentModel.DataAnnotations;

namespace ProductService.Models.DTOs;


public class ProductDto
{
    [Required(ErrorMessage = "Id is required but is not available, data inconsistency.")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive integer.")]
    public int Id { get; set; }
 
    [Required(ErrorMessage = "Product name is required but is not available, data inconsistency.")]
    [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required but is not available, data inconsistency.")]
    [Range(0.01, 100000.00, ErrorMessage = "Price must be between 0.01 and 100,000.00.")]
    public decimal Price { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot be longer than 100 characters.")]
    public string Category { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Stock must be between 0 and 100,000.")]
    public int StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; }
}


public class ProductCreateDto
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters.")]
    public required string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 100000.00, ErrorMessage = "Price must be between 0.01 and 100,000.00.")]
    public required decimal Price { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot be longer than 100 characters.")]
    public string Category { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Stock must be between 0 and 100,000.")]
    public int StockQuantity { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProductUpdateDto
{
    [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 100000.00, ErrorMessage = "Price must be between 0.01 and 100,000.00.")]
    public decimal Price { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot be longer than 100 characters.")]
    public string Category { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "Stock must be between 0 and 100,000.")]
    public int StockQuantity { get; set; } = 0;
}

public class ProductStockUpdateDto
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
    public int StockQuantity { get; set; } = 0;
}
