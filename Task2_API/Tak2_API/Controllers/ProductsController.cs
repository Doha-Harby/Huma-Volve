using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Task2_API.Data;
using Task2_API.DTOs;


namespace Task2_API.Controllers
{

    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/products?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(
                    "pageNumber must be >= 1 and pageSize must be between 1 and 100.");
            }

            var query = _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    RowVersion = p.RowVersion
                });

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                data = products
            });
        }
        // PUT: /api/products/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromBody] UpdateProductDto dto)
        {
            // Validate the category
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest("The specified category does not exist.");
            }

            // Load the entity with tracking enabled (default behavior)
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound("Product not found.");
            }

            // Detect whether the client edited an old version
            _context.Entry(product)
                .Property(p => p.RowVersion)
                .OriginalValue = dto.RowVersion;

            // Change only the allowed fields
            product.Name = dto.Name;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Product updated successfully.",
                    product = new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        CategoryName = (await _context.Categories
                            .Where(c => c.Id == product.CategoryId)
                            .Select(c => c.Name)
                            .FirstAsync()),
                        RowVersion = product.RowVersion
                    }
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                var stillExists = await _context.Products
                    .AnyAsync(p => p.Id == id);

                if (!stillExists)
                {
                    return NotFound("Product was deleted by another operation.");
                }

                return Conflict(new
                {
                    message = "Concurrency conflict. The product was modified by another user. Reload it and try again."
                });
            }
        }

        // DELETE: /api/products/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound("Product not found.");
            }

            product.IsDeleted = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}