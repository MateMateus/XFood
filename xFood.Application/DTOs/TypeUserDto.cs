namespace xFood.Application.DTOs;

// DTO para tipos de usuário (Admin, Manager, User, etc.).
public class TypeUserDto
{
    // Identificador do tipo de usuário.
    public int Id { get; set; }

    // Descrição do tipo.
    public string Description { get; set; } = null!;
}
