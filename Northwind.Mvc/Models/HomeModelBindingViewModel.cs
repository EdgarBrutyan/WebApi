namespace Northwind.Mvc_.Models
{
    public record HomeModelBindingViewModel
    (
        Thing thing,
        bool HasErrors, 
         IEnumerable<string> ValidationError       
    );
   
}
