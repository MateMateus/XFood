using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xFood.Domain.Entities;

// Representa uma categoria de produtos.
public class Category
{
    // Identificador da categoria.
    [Key]
    public int Id { get; set; }

    // Nome da categoria (único no banco).
    [Required, StringLength(80)]
    public string Name { get; set; } = null!;

    // Descrição da categoria.
    [Required, StringLength(500)]
    public string Description { get; set; } = null!;

    // Relação 1:N com produtos.
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
