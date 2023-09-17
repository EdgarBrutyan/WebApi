using Packt.Shared;



namespace Northwind.WebApi.Repositories
{
    public interface IProductRepository
    {
       public Task<Product?> CreateAsync(Product c);
       public Task<IEnumerable<Product>> RetrieveAllAsync();
       public Task<Product?> RetrieveAsync(int id);
       public Task<Product?> UpdateAsync(int id, Product c);
       public Task<bool?> DeleteAsync(int id);
    }
}
