using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Tas1_API_Versioning.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/Products")]
    public class ProductsController : ControllerBase
    {
        private readonly List<Models.Product> _products = new()
        {
            new Models.Product { Id = 1, Name = "Product 1", Price = 10.0m, CategoryName = "Category A" },
            new Models.Product { Id = 2, Name = "Product 2", Price = 20.0m, CategoryName = "Category B" },
            new Models.Product { Id = 3, Name = "Product 3", Price = 30.0m, CategoryName = "Category A" },
            new Models.Product { Id = 4, Name = "Product 4", Price = 40.0m, CategoryName = "Category C" }
        };

        [HttpGet]
        [MapToApiVersion("1.0")]
        public IActionResult GetProductsV1()
        {
           var products = _products.Select(p => new
           {
               p.Id,
               p.Name,
               p.Price
           });
            return Ok(products);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetProductsV2()
        {
            var products = _products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.CategoryName
            });
            return Ok(products);
        }

    }
}
