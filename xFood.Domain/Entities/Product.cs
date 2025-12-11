using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xFood.Domain.Entities;

// Representa um produto do catálogo.
public class Product
{
    // Identificador do produto.
    [Key]
    public int Id { get; set; }

    // Nome do produto.
    [Required, StringLength(80)]
    public string Name { get; set; } = null!;

    // Descrição opcional.
    [StringLength(1000)]
    public string? Description { get; set; }

    // Preço formatado como decimal(18,2).
    [Column(TypeName = "decimal(18,2)")]
    [Required, Range(0, 999.99)]
    public decimal Price { get; set; }

    // Quantidade em estoque.
    [Required, Range(1, int.MaxValue)]
    public int Stock { get; set; }

    // URL da imagem.
    [StringLength(2048)]
    public string? ImageUrl { get; set; }

    // Chave estrangeira da categoria.
    [Required]
    public int CategoryId { get; set; }

    // Navegação para a categoria (1:N).
    public virtual Category? Category { get; set; }
}
