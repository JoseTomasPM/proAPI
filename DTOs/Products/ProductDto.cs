using System.ComponentModel.DataAnnotations;

namespace ProfessionalAPI.DTOs.Products;

public record ProductDto(int Id, string Name, string Description, decimal Price, int Stock);

public record CreateProductDto(
    [Required][MaxLength(100)] string Name,
    [MaxLength(500)] string Description,
    [Range(0.01, double.MaxValue)] decimal Price,
    [Range(0, int.MaxValue)] int Stock
);

public record UpdateProductDto(
    [MaxLength(100)] string? Name,
    [MaxLength(500)] string? Description,
    [Range(0.01, double.MaxValue)] decimal? Price,
    [Range(0, int.MaxValue)] int? Stock
);