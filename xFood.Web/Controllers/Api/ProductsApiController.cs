using Microsoft.AspNetCore.Mvc;

using xFood.Application.DTOs;
using xFood.Application.Interfaces;

namespace xFood.Web.Controllers.Api
{
    [ApiController]
    [Route("api/products")]
    /// <summary>
    /// API de produtos com endpoints de consulta, criação, edição e exclusão.
    /// </summary>
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;

        public ProductsApiController(IProductRepository products, ICategoryRepository categories)
        {
            _products = products;
            _categories = categories;
        }

        // GET api/products?categoryId=&q=&page=1&size=12
        /// <summary>
        /// Lista produtos com paginação e filtros por categoria e termo.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] string? q,
                                                [FromQuery] int page = 1, [FromQuery] int size = 12)
        {
            var (total, items) = await _products.GetAllAsync(categoryId, q, page, size);
            return Ok(new { total, page, size, items });
        }

        // GET api/products/5
        /// <summary>
        /// Obtém um produto específico por Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var dto = await _products.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST api/products
        /// <summary>
        /// Cria um novo produto após validar a categoria informada.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var cats = await _categories.GetAllAsync();
            if (!cats.Any(c => c.Id == dto.CategoryId))
                return BadRequest("Categoria inválida.");

            var id = await _products.CreateAsync(dto);
            var created = await _products.GetByIdAsync(id);
            return CreatedAtAction(nameof(GetById), new { id }, created);
        }

        // PUT api/products/5
        /// <summary>
        /// Atualiza um produto existente, validando a categoria.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCreateUpdateDto dto)
        {
            var cats = await _categories.GetAllAsync();
            if (!cats.Any(c => c.Id == dto.CategoryId))
                return BadRequest("Categoria inválida.");

            await _products.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE api/products/5
        /// <summary>
        /// Exclui um produto por Id.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _products.DeleteAsync(id);
            return NoContent();
        }
    }
}
