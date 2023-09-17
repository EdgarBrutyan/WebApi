using Microsoft.EntityFrameworkCore; // UseSqlite
using Microsoft.Extensions.DependencyInjection; // IServiceCollection
using System.Data.SqlTypes;
namespace Packt.Shared;
public static class NorthwindContextExtensions
{
 public static IServiceCollection AddNorthwindContext(
 this IServiceCollection services, string relativePath = "..")
    {
        string databasePath = Path.Combine(relativePath, "Northwind.db");
        services.AddDbContext<Nortwind>(options =>
        options.UseSqlServer($"Data Source={databasePath}")
        );
        return services;
    }
}