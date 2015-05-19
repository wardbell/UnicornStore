using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.AspNet.Controllers
{
    //Todo: move all Entity Framework interaction to a repository/unit-of-work

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

        [HttpGet]
        // IQueryable doesn't really work. 
        // This filters nothing: http://localhost:5000/api/products?$filter=ProductId%20eq%202
        public IQueryable<Product> Products() 
        {
            return db.Products;
        }

        [HttpGet("{id:int}", Name = "ProductsGetByIdRoute")]
        public IActionResult GetById(int id)
        {
            var product = db.Products.FirstOrDefault(c => c.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return new ObjectResult(product);
        }

        [HttpGet("First3Products")]
        public IEnumerable<Product> First3Products()
        {
            return db.Products.Take(3);
        }

        [HttpGet("Summaries")]
        public IEnumerable<object> Summaries()
        {
            return db.Products.Select(p => new { p.ProductId, p.DisplayName, p.CurrentPrice, p.MSRP });
        }

        [HttpGet("ByCategory/{id:int}")]
        public IEnumerable<Product> ProductsByCategory(int id)
        {
            return db.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category);
        }


        [HttpPost]
        //[Authorize] // A MUST ... but leaving out for demo purposes
        //[ValidateAntiForgeryToken] // Todo: support these tokens to prevent XSRF
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest();
            }
            else
            {
                db.Products.Add(product);
                db.SaveChanges();

                string url = Url.RouteUrl("ProductsGetByIdRoute", new { id = product.ProductId },
                    Request.Scheme, Request.Host.ToUriComponent());

                Context.Response.StatusCode = 201;
                Context.Response.Headers["Location"] = url;
                return new ObjectResult(product);
            }
        }

        [HttpPut]
        //[Authorize] // A MUST ... but leaving out for demo purposes
        //[ValidateAntiForgeryToken]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest();
            }

            var origProduct = db.Products.AsNoTracking().FirstOrDefault(x => x.ProductId == product.ProductId);
            if (origProduct == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Todo: validation ... referencing the original product as necessary
                db.Products.Update(product);
                db.SaveChanges();
                return new ObjectResult(product);
            }
        }

        [HttpDelete("{id:int}")]
        //[Authorize] // A MUST ... but leaving out for demo purposes
        //[ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= MAX_ORIGINAL_ID)
            {
                Context.Response.StatusCode = 400;
                return new ObjectResult("You may not delete one of the original UnicornStore products");
            }

            var product = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return new HttpStatusCodeResult(204); // No Content
        }

        private const int MAX_ORIGINAL_ID = 7;
    }
}
