
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packt.Shared
{ 
    public class Nortwind : DbContext
    {
        public DbSet<Category>? Categories { get; set; }
        public DbSet <Product>? Products { get; set; }


        public Nortwind(DbContextOptions<Nortwind> options) : base(options)
        {
        }

        protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
        {
            string connection = "Data Source=.;" +
            "Initial Catalog=Northwind;" +
            "Integrated Security=true;" +
            "MultipleActiveResultSets=true;";
            optionsBuilder.UseSqlServer(connection);
        } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<Product>().HasQueryFilter(e => e.ProductName.Contains("L"));
        }
    }


    public class Category
    {
        Category()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryId  { get; set; }

        [Required]
        public string? CategoryName { get; set; }

        [Column (TypeName = "ntext")]
        public string? Description { get; set; }

      //  [InverseProperty(nameof(Product.Category))]
        public virtual ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        [Key]
        public int ProductId { get; set; } // первичный ключ

        [Required]
        [StringLength(40)]
        public string ProductName { get; set; } = null!;

        [Column("UnitPrice", TypeName = "money")]
        public decimal? Cost { get; set; } // имя свойства != имя столбца
   
        [Column("UnitsInStock")]
        public short? Stock { get; set; }

        public bool Discontinued { get; set; }
        // эти два параметра определяют отношение внешнего ключа
        // к таблице Categories
        public int CategoryId { get; set; }

      //  public virtual Category Category { get; set; } = null!;
    }

}
