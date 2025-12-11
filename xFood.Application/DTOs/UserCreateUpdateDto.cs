using System.ComponentModel.DataAnnotations;

namespace xFood.Application.DTOs;

// Modelo para criação e edição de usuários.
public class UserCreateUpdateDto
{
    // Nome completo.
    [Required, StringLength(150)]
    public string Name { get; set; } = null!;

    // E-mail do usuário.
    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;

    // Senha do usuário (ideal usar hash em produção).
    [Required, StringLength(150)]
    public string Password { get; set; } = null!;

    // Data de nascimento.
    [Required]
    [Display(Name = "Data de Nascimento")]
    public DateTime DateBirth { get; set; }

    // Tipo de usuário (perfil).
    [Required]
    [Display(Name = "Tipo de Usuário")]
    public int TypeUserId { get; set; }

    // Status ativo/inativo (padrão: ativo).
    [Display(Name = "Ativo")]
    public bool Active { get; set; } = true;
}
