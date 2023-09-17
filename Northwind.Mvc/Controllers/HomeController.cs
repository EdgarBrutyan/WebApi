using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc_.Models;
using System.Diagnostics;
using Packt.Shared;
using Northwind.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Collections.Generic;

namespace Northwind.Mvc_.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly Nortwind _db;

        public HomeController(ILogger<HomeController> logger, Nortwind db, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _db = db;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Product(int id)
        {
            string uri;
           
            ViewData["Title"] = $"Customers in {id}";
            uri = $"https://localhost:5002/api/customers/?id={id}";
            

            HttpClient client = _clientFactory.CreateClient(name: "Northwind.WebApi");
            HttpRequestMessage request = new(
            method: HttpMethod.Get, requestUri: uri);
            HttpResponseMessage response = await client.SendAsync(request);
            IEnumerable<Product>? model = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return View(model);
        }


        public IActionResult ProductThatCostMoreThan(decimal? price)
        {
            if(!price.HasValue)
            {
                return BadRequest("Not Ok");
            }

            IEnumerable<Product> model = _db.Products.Where(p => p.Cost >= price);

            if(!model.Any())
            {
                return NotFound($"There is no any product with price more than {price}");
            }

            ViewData["MaxPrice"] = price.Value.ToString("C");
            return View(model);
        }

        public IActionResult ModelBinding()
        {
            return View(); // страница с формой
        }

        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new(
            thing,
            !ModelState.IsValid,
            ModelState.Values
            .SelectMany(state => state.Errors)
            .Select(error => error.ErrorMessage)
            );
            return View(model);
        }

        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            _logger.LogError("This is a serious error (not really!)");
            _logger.LogWarning("This is your first warning!");
            _logger.LogWarning("Second warning!");
            _logger.LogInformation("I am in the Index method of the HomeController.");

            HomeIndexViewModel model = new
            (
                VisitorCount: (new Random()).Next(1, 1001),
                Product: await _db.Products.ToListAsync()
            );
            return View(model);
        }

        [Route("privacy")]
        [Authorize (Roles="Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ProductDetail(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the route, for example, / Home / ProductDetail / 21");
            }
            Product? model = _db.Products
            .SingleOrDefault(p => p.ProductId == id);
            if (model == null)
            {
                return NotFound($"ProductId {id} not found.");
            }
            return View(model); // передаем модель для просмотра и возвращаем результат
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}