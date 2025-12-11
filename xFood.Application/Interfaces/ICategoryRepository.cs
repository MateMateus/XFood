using xFood.Application.DTOs;

namespace xFood.Application.Interfaces;

// Interface que define o contrato para manipulação de categorias.
// Aqui ficam apenas as assinaturas dos métodos (sem lógica).
public interface ICategoryRepository
{
    // Retorna a lista completa de categorias, já ordenadas por nome.
    // Quem implementar precisa buscar no banco e devolver o resultado.
    Task<List<CategoryDto>> GetAllAsync();

    // Busca uma única categoria pelo Id.
    // Retorna null se não encontrar.
    Task<CategoryDto?> GetByIdAsync(int id);

    // Verifica se a categoria possui produtos associados.
    // Usado para impedir exclusões indevidas.
    Task<bool> AnyProductsAsync(int categoryId);

    // Cria uma nova categoria no banco de dados.
    // Recebe o DTO com os dados e retorna o Id gerado.
    Task<int> CreateAsync(CategoryDto dto);

    // Atualiza os dados de uma categoria existente.
    // A implementação deve validar e aplicar as alterações.
    Task UpdateAsync(CategoryDto dto);

    // Remove uma categoria pelo Id.
    // Quem implementar deve garantir que a operação é segura
    // (ex: verificar antes se existem produtos vinculados).
    Task DeleteAsync(int id);
}
