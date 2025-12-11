namespace xFood.Application.DTOs;

// DTO de leitura de produto, incluindo dados da categoria.
public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    string? ImageUrl,
    int CategoryId,
    string? CategoryName
);
