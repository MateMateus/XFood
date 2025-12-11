using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using xFood.Domain.Entities;
using xFood.Infrastructure.Persistence;

namespace xFood.Infrastructure.Seeding;

public static class SeedService
{
    public static async Task EnsureSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<xFoodDbContext>();

        // Cria/Migra o banco na primeira execução
        await ctx.Database.MigrateAsync();

        // ============================
        // Tipos de Usuário
        // ============================
        if (!await ctx.TypeUsers.AnyAsync())
        {
            var tipos = new[]
            {
                new TypeUser { Description = "Usuário" },
                new TypeUser { Description = "Administrador" },
                new TypeUser { Description = "Gerente" }
            };

            ctx.TypeUsers.AddRange(tipos);
            await ctx.SaveChangesAsync();
        }

        // ============================
        // Categorias
        // ============================
        if (!await ctx.Categories.AnyAsync())
        {
            var categorias = new[]
            {
                new Category { Name = "Lanche",    Description = "Lanches e sanduíches" },
                new Category { Name = "Bebida",    Description = "Bebidas diversas" },
                new Category { Name = "Sobremesa", Description = "Doces e sobremesas" }
            };

            ctx.Categories.AddRange(categorias);
            await ctx.SaveChangesAsync();
        }

        // ============================
        // Produtos (10 por categoria)
        // ============================
        if (!await ctx.Products.AnyAsync())
        {
            var catDict = await ctx.Categories
                .AsNoTracking()
                .ToDictionaryAsync(c => c.Name, c => c.Id);

            var produtos = new List<Product>();

            // ------ Lanches ------
            var lancheId = catDict["Lanche"];
            produtos.AddRange(new[]
            {
                P("X-Burger",            "Pão, hambúrguer, queijo e molho da casa", 22.90m, 50, lancheId),
                P("X-Salada",            "Pão, hambúrguer, queijo, alface e tomate", 24.90m, 40, lancheId),
                P("X-Bacon",             "Hambúrguer com bacon crocante",            28.90m, 35, lancheId),
                P("Frango Crispy",       "Sanduíche de frango empanado",             26.90m, 30, lancheId),
                P("Cheddar Melt",        "Hambúrguer com cheddar cremoso",           29.90m, 25, lancheId),
                P("Veggie Burger",       "Grão de bico e legumes grelhados",         27.90m, 20, lancheId),
                P("Dog Tradicional",     "Salsicha, purê, batata palha",             18.90m, 50, lancheId),
                P("Dog Duplo",           "Duas salsichas e queijo",                   22.90m, 40, lancheId),
                P("Wrap de Frango",      "Frango, alface, tomate e molho",            23.90m, 30, lancheId),
                P("Wrap Veggie",         "Legumes grelhados e hummus",                23.90m, 25, lancheId),
            });

            // ------ Bebidas ------
            var bebidaId = catDict["Bebida"];
            produtos.AddRange(new[]
            {
                P("Refrigerante Lata",   "350 ml",                                   6.50m, 120, bebidaId),
                P("Suco de Laranja",     "Natural 300 ml",                           9.90m,  60, bebidaId),
                P("Água Mineral",        "Sem gás 500 ml",                           4.50m, 150, bebidaId),
                P("Água com Gás",        "Com gás 500 ml",                           5.00m, 100, bebidaId),
                P("Chá Gelado",          "Pêssego 300 ml",                           7.90m,  80, bebidaId),
                P("Guaraná 600 ml",      "Garrafa",                                  8.90m,  70, bebidaId),
                P("Coca-Cola 600 ml",    "Garrafa",                                  9.90m,  70, bebidaId),
                P("Suco de Uva",         "Integral 300 ml",                          10.90m, 50, bebidaId),
                P("Limonada",            "300 ml",                                   7.50m,  60, bebidaId),
                P("Café Gelado",         "Doce na medida 300 ml",                    11.90m, 40, bebidaId),
            });

            // ------ Sobremesas ------
            var sobremesaId = catDict["Sobremesa"];
            produtos.AddRange(new[]
            {
                P("Brownie",             "Chocolate com nozes",                      12.90m, 30, sobremesaId),
                P("Cookie",              "Gotas de chocolate",                        6.90m,  80, sobremesaId),
                P("Cheesecake",          "Calda de frutas vermelhas",                14.90m, 25, sobremesaId),
                P("Pudim",               "Leite condensado",                          9.90m,  40, sobremesaId),
                P("Mousse de Maracujá",  "Cremoso",                                   9.90m,  35, sobremesaId),
                P("Torta de Limão",      "Massa crocante e creme de limão",          13.90m, 20, sobremesaId),
                P("Açaí 300 ml",         "Com granola",                              15.90m, 30, sobremesaId),
                P("Sorvete 2 bolas",     "Sabores do dia",                           11.90m, 50, sobremesaId),
                P("Petit Gâteau",        "Bolo quente + sorvete",                    18.90m, 15, sobremesaId),
                P("Romeu e Julieta",     "Goiabada e queijo minas",                  10.90m, 25, sobremesaId),
            });

            ctx.Products.AddRange(produtos);
            await ctx.SaveChangesAsync();
        }

        // ============================
        // Usuários (Admin, Gerente, Usuário)
        // ============================
        if (!await ctx.Users.AnyAsync())
        {
            // Descobre os Ids reais dos perfis (evita assumir 1/2/3)
            var roles = await ctx.TypeUsers.AsNoTracking()
                .ToDictionaryAsync(t => t.Description, t => t.Id);

            var adminId = roles["Administrador"];
            var gerenteId = roles["Gerente"];
            var userId = roles["Usuário"];

            ctx.Users.AddRange(
                new User
                {
                    Name = "Administrador",
                    Email = "admin@xfood.com",
                    Password = "123",          // simples para estudo
                    DateBirth = new DateTime(1990, 1, 1),
                    TypeUserId = adminId,
                    Active = true
                },
                new User
                {
                    Name = "Gerente",
                    Email = "gerente@xfood.com",
                    Password = "123",
                    DateBirth = new DateTime(1995, 1, 1),
                    TypeUserId = gerenteId,
                    Active = true
                },
                new User
                {
                    Name = "Usuário",
                    Email = "usuario@xfood.com",
                    Password = "123",
                    DateBirth = new DateTime(2000, 1, 1),
                    TypeUserId = userId,
                    Active = true
                }
            );

            await ctx.SaveChangesAsync();
        }
    }

    // Helper para instanciar Produto
    private static Product P(string name, string desc, decimal price, int stock, int categoryId) =>
        new()
        {
            Name = name,
            Description = desc,
            Price = price,
            Stock = stock,
            ImageUrl = null,
            CategoryId = categoryId
        };
}
