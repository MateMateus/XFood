using Microsoft.EntityFrameworkCore;
using xFood.Application.DTOs;
using xFood.Application.Interfaces;
using xFood.Domain.Entities;
using xFood.Infrastructure.Persistence;

namespace xFood.Infrastructure.Repositories;

// Repositório para operações com usuários (CRUD, filtros e status).
public class UserRepository : IUserRepository
{
    private readonly xFoodDbContext _ctx;
    public UserRepository(xFoodDbContext ctx) => _ctx = ctx;

    // Retorna todos os usuários como DTOs.
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await _ctx.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                DateBirth = u.DateBirth,
                Active = u.Active,
                TypeUserId = u.TypeUserId,
                TypeUserDescription = u.TypeUser != null ? u.TypeUser.Description : null
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    // Busca usuário pelo Id.
    public async Task<UserDto?> GetByIdAsync(int id)
    {
        return await _ctx.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                DateBirth = u.DateBirth,
                Active = u.Active,
                TypeUserId = u.TypeUserId,
                TypeUserDescription = u.TypeUser != null ? u.TypeUser.Description : null
            })
            .FirstOrDefaultAsync();
    }

    // Cria novo usuário e retorna o Id.
    public async Task<int> CreateAsync(UserCreateUpdateDto dto)
    {
        var entity = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password, // sem hash (didático)
            DateBirth = dto.DateBirth,
            TypeUserId = dto.TypeUserId,
            Active = dto.Active
        };

        _ctx.Users.Add(entity);
        await _ctx.SaveChangesAsync();
        return entity.Id;
    }

    // Atualiza usuário existente.
    public async Task UpdateAsync(int id, UserCreateUpdateDto dto)
    {
        var entity = await _ctx.Users.FindAsync(id);
        if (entity is null) return;

        entity.Name = dto.Name;
        entity.Email = dto.Email;
        entity.Password = dto.Password;
        entity.DateBirth = dto.DateBirth;
        entity.TypeUserId = dto.TypeUserId;
        entity.Active = dto.Active;

        await _ctx.SaveChangesAsync();
    }

    // Remove usuário permanentemente.
    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Users.FindAsync(id);
        if (entity is null) return;

        _ctx.Users.Remove(entity);
        await _ctx.SaveChangesAsync();
    }

    // Filtra usuários por status (ativos, inativos ou todos).
    public async Task<IEnumerable<UserDto>> GetByFilterAsync(bool? active)
    {
        var q = _ctx.Users.AsNoTracking();

        if (active.HasValue)
            q = q.Where(u => u.Active == active.Value);

        return await q
            .OrderBy(u => u.Name)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                DateBirth = u.DateBirth,
                Active = u.Active,
                TypeUserId = u.TypeUserId,
                TypeUserDescription = u.TypeUser != null ? u.TypeUser.Description : null
            })
            .ToListAsync();
    }

    // Soft delete — marca como inativo.
    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _ctx.Users.FindAsync(id);
        if (entity is null) return;

        entity.Active = false;
        await _ctx.SaveChangesAsync();
    }

    // Define explicitamente o status ativo/inativo.
    public async Task SetActiveAsync(int id, bool active)
    {
        var entity = await _ctx.Users.FindAsync(id);
        if (entity is null) return;

        entity.Active = active;
        await _ctx.SaveChangesAsync();
    }
}
