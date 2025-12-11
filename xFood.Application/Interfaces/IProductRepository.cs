using xFood.Application.DTOs;

namespace xFood.Application.Interfaces;

// Contrato para operações paginadas e CRUD de produtos.
public interface IProductRepository
{
    // Retorna produtos paginados com filtros opcionais.
    Task<(int total, List<ProductDto> items)> GetAllAsync(int? categoryId, string? q, int page, int size);

    // Obtém um produto pelo Id.
    Task<ProductDto?> GetByIdAsync(int id);

    // Cria um novo produto e retorna seu Id.
    Task<int> CreateAsync(ProductCreateUpdateDto dto);

    // Atualiza um produto existente.
    Task UpdateAsync(int id, ProductCreateUpdateDto dto);

    // Remove um produto pelo Id.
    Task DeleteAsync(int id);
}
