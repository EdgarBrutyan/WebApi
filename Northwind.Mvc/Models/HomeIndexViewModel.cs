using Packt.Shared; // Category, Product
namespace Northwind.Mvc.Models;
public record HomeIndexViewModel
(
 int VisitorCount,
 IList<Product> Product
);
