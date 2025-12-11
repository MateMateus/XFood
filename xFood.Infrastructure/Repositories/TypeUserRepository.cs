using Microsoft.EntityFrameworkCore;
using xFood.Application.DTOs;
using xFood.Application.Interfaces;
using xFood.Infrastructure.Persistence;

namespace xFood.Infrastructure.Repositories;

// Repositório para leitura de tipos de usuário.
public class TypeUserRepository : ITypeUserRepository
{
    private readonly xFoodDbContext _ctx;
    public TypeUserRepository(xFoodDbContext ctx) => _ctx = ctx;

    // Retorna todos os tipos de usuário ordenados por descrição.
    public async Task<IEnumerable<TypeUserDto>> GetAllAsync()
    {
        return await _ctx.TypeUsers
            .AsNoTracking()
            .OrderBy(t => t.Description)
            .Select(t => new TypeUserDto
            {
                Id = t.Id,
                Description = t.Description
            })
            .ToListAsync();
    }
}
