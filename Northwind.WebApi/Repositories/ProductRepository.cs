using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using Packt.Shared; // Customer
using System.Collections.Concurrent;

namespace Northwind.WebApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private static ConcurrentDictionary<int, Product>? customersCache;

        private Nortwind db;

        public ProductRepository(Nortwind injectedContext)
        {
            db = injectedContext;
            // предварительно загружаем клиентов из базы данных как обычный 
            // словарь с идентификатором клиента в качестве ключа,
            // затем преобразуем в потокобезопасный ConcurrentDictionary
            if (customersCache is null)
            {
                customersCache = new ConcurrentDictionary<int, Product>(
                db.Products.ToDictionary(c => c.ProductId));
            }
        }

        public async Task<Product?> CreateAsync(Product c)
        {
            // добавляем в базу данных с помощью EF Core
            EntityEntry<Product> added = await db.Products.AddAsync(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (customersCache is null) return c;
                // нового клиента добавляем в кэш, иначе вызываем метод UpdateCache
                return customersCache.AddOrUpdate(c.ProductId, c, UpdateCache);
            }
            else
            {
                return null;
            }

        }

        public Task<IEnumerable<Product>> RetrieveAllAsync()
        {
            // в целях производительности извлекаем из кэша
            return Task.FromResult(customersCache is null
            ? Enumerable.Empty<Product>() : customersCache.Values);
        }

        public Task<Product?> RetrieveAsync(int id)
        {
            if (customersCache is null) return null!;
            customersCache.TryGetValue(id, out Product? c);
            return Task.FromResult(c);
        }

        private Product UpdateCache(int id, Product c)
        {
            Product? old;
            if (customersCache is not null)
            {
                if (customersCache.TryGetValue(id, out old))
                {
                    if (customersCache.TryUpdate(id, c, old))
                    {
                        return c;
                    }
                }
            }
            return null!;
        }

        public async Task<Product?> UpdateAsync(int id, Product c)
        {
            // нормализуем идентификатор клиента

            // обновляем в базе
            db.Products.Update(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // обновляем в кэше
                return UpdateCache(id, c);
            }

            return null;
        }

        public async Task<bool?> DeleteAsync(int id)
        {
            // удаляем из базы данных
            Product? c = db.Products.Find(id);
            if (c is null) return null;
            db.Products.Remove(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (customersCache is null) return null;
                // удаляем из кэша
                return customersCache.TryRemove(id, out c);
            }
            else
            {
                return null;
            }
        }
    }
}


