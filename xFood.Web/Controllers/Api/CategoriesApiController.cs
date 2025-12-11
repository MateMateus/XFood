using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using xFood.Domain.Entities;
using xFood.Infrastructure.Persistence;

namespace xFood.Web.Controllers.Api
{
    [ApiController]
    [Route("api/categories")] // /api/categories
    [Produces("application/json")]
    /// <summary>
    /// API de categorias com endpoints de listagem e CRUD simples.
    /// </summary>
    public class CategoriesApiController : ControllerBase
    {
        private readonly xFoodDbContext _ctx;
        public CategoriesApiController(xFoodDbContext ctx) => _ctx = ctx;

        // GET: /api/categories
        /// <summary>
        /// Retorna todas as categorias ordenadas por nome.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var items = await _ctx.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(items);
        }

        // GET: /api/categories/5
        /// <summary>
        /// Retorna dados completos de uma categoria por Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var entity = await _ctx.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity is null) return NotFound();
            return Ok(entity);
        }

        // POST: /api/categories
        /// <summary>
        /// Cria uma nova categoria; evita duplicidade por nome.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category model)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // opcional: evitar duplicadas
            var exists = await _ctx.Categories.AnyAsync(c => c.Name == model.Name);
            if (exists) return Conflict("Já existe uma categoria com esse nome.");

            _ctx.Categories.Add(model);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // PUT: /api/categories/5
        /// <summary>
        /// Atualiza categoria existente, respeitando o Id do recurso.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category model)
        {
            if (id != model.Id) return BadRequest("Id do recurso difere do corpo da requisição.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            _ctx.Entry(model).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _ctx.Categories.AnyAsync(c => c.Id == id);
                if (!exists) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: /api/categories/5
        /// <summary>
        /// Exclui uma categoria por Id.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _ctx.Categories.FindAsync(id);
            if (entity is null) return NotFound();

            // Se quiser impedir exclusão com produtos vinculados, descomente:
            // var hasProducts = await _ctx.Products.AnyAsync(p => p.CategoryId == id);
            // if (hasProducts) return BadRequest("Categoria possui produtos vinculados.");

            _ctx.Categories.Remove(entity);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}
