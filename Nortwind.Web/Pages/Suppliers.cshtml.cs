using Microsoft.AspNetCore.Mvc.RazorPages; // PageModel
using Packt.Shared;

namespace Northwind.Web.Pages;
using Microsoft.AspNetCore.Mvc;
public class SuppliersModel : PageModel
{
    private Nortwind db;
        
    public SuppliersModel(Nortwind injectedContext)
    {
        db = injectedContext;
    }

    public IEnumerable<Product>? Products { get; set; }
    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Suppliers";
        Products = db.Products
   .OrderBy(c => c.ProductId).ThenBy(c => c.ProductName);
    }


    [BindProperty]
    public Product? product { get; set; }
    public IActionResult OnPost()
    {
        if ((product is not null) && ModelState.IsValid)
        {
            db.Products.Add(product);
            db.SaveChanges();
            return RedirectToPage("/Suppliers");
        }
        else
        {
            Console.WriteLine("No");
            return Page(); // возвращает исходную страницу
        } 
    }
}