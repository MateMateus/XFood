using System.ComponentModel.DataAnnotations;

namespace xFood.Application.DTOs;

// DTO de leitura para usuários, incluindo dados básicos e de perfil.
public class UserDto
{
    // Identificador do usuário.
    public int Id { get; set; }

    // Nome completo.
    public string Name { get; set; } = null!;

    // E-mail do usuário.
    public string Email { get; set; } = null!;

    // Data de nascimento.
    public DateTime DateBirth { get; set; }

    // Indica se o usuário está ativo.
    public bool Active { get; set; }

    // Id do tipo de usuário.
    public int TypeUserId { get; set; }

    // Descrição do tipo de usuário (se carregado).
    public string? TypeUserDescription { get; set; }
}
