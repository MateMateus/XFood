using xFood.Application.DTOs;

namespace xFood.Application.Interfaces;

// Contrato para operações com usuários, incluindo filtros e soft-delete.
public interface IUserRepository
{
    // Retorna todos os usuários.
    Task<IEnumerable<UserDto>> GetAllAsync();

    // Obtém um usuário pelo Id.
    Task<UserDto?> GetByIdAsync(int id);

    // Cria um novo usuário e retorna seu Id.
    Task<int> CreateAsync(UserCreateUpdateDto dto);

    // Atualiza os dados de um usuário existente.
    Task UpdateAsync(int id, UserCreateUpdateDto dto);

    // Remove um usuário permanentemente (hard delete).
    Task DeleteAsync(int id);

    // Consulta usuários por status: ativos, inativos ou todos.
    Task<IEnumerable<UserDto>> GetByFilterAsync(bool? active);

    // Marca o usuário como inativo (soft delete).
    Task SoftDeleteAsync(int id);

    // Define explicitamente o status ativo/inativo.
    Task SetActiveAsync(int id, bool active);
}
