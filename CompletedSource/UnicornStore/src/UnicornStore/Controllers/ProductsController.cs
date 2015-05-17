using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.AspNet.Controllers
{

    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private IUnicornStoreContext db;
        private IFoo Foo {get;}
        private MvcOptions MvcOptions { get; set; }

        public ProductsController(IUnicornStoreContext dbContext, IFoo foo)
        {
            db = dbContext;
            Foo = foo;
        }

        //[HttpGet]
        [Route("")]
        // IQueryable doesn't really work. 
        // This filters nothing: http://localhost:5000/api/products?$filter=ProductId%20eq%202
        public IQueryable<Product> Products() 
        {
            return db.Products;
        }
        [Route("First3Products")]
        public IEnumerable<Product> First3Products()
        {
            return db.Products.Take(3);
        }

        [Route("ByCategory/{id}")]
        public IEnumerable<Product> ProductsByCategory(int id)
        {
            return db.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category);
        }
    }
}
