using xFood.Application.DTOs;

namespace xFood.Application.Interfaces;

// Contrato para consultas de tipos de usuário.
public interface ITypeUserRepository
{
    // Retorna todos os tipos de usuário.
    Task<IEnumerable<TypeUserDto>> GetAllAsync();
}
