using Microsoft.AspNetCore.Mvc; // [Route], [ApiController],
using Packt.Shared; // Customer
using Northwind.WebApi.Repositories;


namespace Northwind.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository repo;
        private readonly ILogger<ProductController> _loger;


        public ProductController(IProductRepository repo, ILogger<ProductController> loger)
        {
            this.repo = repo;
            _loger = loger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        public async Task<IEnumerable<Product>> GetProduct()
        {
            return (await repo.RetrieveAllAsync());   
        }


        [HttpGet("{id}", Name = nameof(GetProduct))] // именованный маршрут
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProduct(int id)
        {
            Product? c = await repo.RetrieveAsync(id);
            if (c == null)
            {
                return NotFound(); // 404 – ресурс не найден
            }
            return Ok(c); // 200 – OK, с клиентом в теле
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Product c)
        {
            if (c == null)
            {
                return BadRequest(); // 400 – некорректный запрос
            }
            Product? addedCustomer = await repo.CreateAsync(c);
            if (addedCustomer == null)
            {
                Console.WriteLine("Not OKAY");
                return BadRequest("Repository failed to create customer.");
            }
            else
            {
                return CreatedAtRoute( // 201 – ресурс создан
                routeName: nameof(GetProduct),
                routeValues: new { id = addedCustomer.ProductId },
                value: addedCustomer);
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] Product c)
        {
            if (c == null || c.ProductId != id)
            {
                return BadRequest(); // 400 – некорректный запрос
            }
            Product? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 – ресурс не найден
            }
            await repo.UpdateAsync(id, c);
            return new NoContentResult(); // 204 – контент отсутствует
        }


        // DELETE: api/customers/[id]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                ProblemDetails problemDetails = new()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://localhost:5001/customers/failed-to-delete",
                    Title = $"Customer ID {id} found but failed to delete.",
                    Detail = "More details like Company Name, Country and so on.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails); // 400 – некорректный запрос
            }


            Product? existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 – ресурс не найден
            }
            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value) // короткое замыкание AND
            {
                return new NoContentResult(); // 204 – контент отсутствует
            }
            else
            {
                return BadRequest( // 400 – некорректный запрос
                $"Customer {id} was found but failed to delete.");
            }
        }
    }
}
