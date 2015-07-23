using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using UnicornStore.AspNet.Models.UnicornStore;
using Microsoft.Data.Entity;

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
        // IQueryable doesn't work; treated as IEnumerable 
        // This filters nothing: http://localhost:5000/api/products?$filter=ProductId%20eq%202
        // Todo: make async like the other methods of this controller.
        // [EnableQueryable] // doesn't exist (yet)
        public IQueryable<Product> Products() 
        {
            return db.Products;
        }

        [HttpGet("{id:int}", Name = "ProductsGetByIdRoute")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await db.Products.FirstOrDefaultAsync(c => c.ProductId == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return new ObjectResult(product);
        }

        [HttpGet("First3Products")]
        public async Task<IEnumerable<Product>> First3Products()
        {
            return await db.Products.Take(3).ToListAsync();
        }

        [HttpGet("Summaries")]
        public async Task<IEnumerable<object>> Summaries()
        {
            return await db.Products
                .Select(p => new { p.ProductId, p.DisplayName, p.CurrentPrice, p.MSRP })
                .ToListAsync();
        }

        [HttpGet("ByCategory/{id:int}")]
        public async Task<IEnumerable<Product>> ProductsByCategory(int id)
        {
            return await db.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category)
                .ToListAsync();
        }


        [HttpPost]
        //[Authorize] // A MUST ... but leaving out for demo purposes
        //[ValidateAntiForgeryToken] // Todo: support these tokens to prevent XSRF
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest();
            }
            else
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();

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
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
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
                await db.SaveChangesAsync();
                return new ObjectResult(product);
            }
        }

        [HttpDelete("{id:int}")]
        //[Authorize] // A MUST ... but leaving out for demo purposes
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
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
            await db.SaveChangesAsync();
            return new HttpStatusCodeResult(204); // No Content
        }

        private const int MAX_ORIGINAL_ID = 7;
    }
}
