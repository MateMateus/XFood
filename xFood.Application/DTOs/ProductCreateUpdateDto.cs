using System.ComponentModel.DataAnnotations;

namespace xFood.Application.DTOs;

// Modelo para criação e edição de produtos.
public class ProductCreateUpdateDto
{
    // Nome do produto.
    [Required, StringLength(120)]
    public string Name { get; set; } = null!;

    // Descrição opcional.
    [StringLength(1000)]
    public string? Description { get; set; }

    // Preço do produto.
    [Range(0, 9999999)]
    public decimal Price { get; set; }

    // Quantidade em estoque.
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    // URL da imagem.
    public string? ImageUrl { get; set; }

    // Id da categoria do produto.
    [Required]
    public int CategoryId { get; set; }
}
