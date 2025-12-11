using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xFood.Domain.Entities;

// Tipo de usuário (perfil), como Admin, Manager, User.
public class TypeUser
{
    // Identificador do tipo de usuário.
    [Key]
    public int Id { get; set; }

    // Descrição do perfil.
    [Required, StringLength(150)]
    public string Description { get; set; } = null!;

    // Relação 1:N com usuários.
    public ICollection<User> Users { get; set; } = new List<User>();
}
