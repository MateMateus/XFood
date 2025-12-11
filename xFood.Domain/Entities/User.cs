using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xFood.Domain.Entities;

// Usuário do sistema com dados básicos e vínculo de tipo.
public class User
{
    // Identificador do usuário.
    [Key]
    public int Id { get; set; }

    // Nome completo.
    [Required, StringLength(150)]
    public string Name { get; set; } = null!;

    // E-mail do usuário.
    [Required, StringLength(150)]
    public string Email { get; set; } = null!;

    // Senha em texto simples (apenas didático).
    [Required, StringLength(12)]
    public string Password { get; set; } = null!;

    // Data de nascimento.
    [Required]
    public DateTime DateBirth { get; set; }

    // Chave estrangeira do tipo de usuário.
    [Display(Name = "User Type")]
    public int TypeUserId { get; set; }

    // Navegação para o tipo de usuário.
    public virtual TypeUser? TypeUser { get; set; }

    // Indica se o usuário está ativo.
    public bool Active { get; set; } = true;
}
