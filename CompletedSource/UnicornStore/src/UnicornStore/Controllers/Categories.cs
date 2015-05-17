using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.AspNet.Controllers
{

    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private IUnicornStoreContext db;

        public CategoriesController(IUnicornStoreContext dbContext)
        {
            db = dbContext;
        }

        //[HttpGet]
        [Route("")]
        public IEnumerable<Category> Category() 
        {
            return db.Categories;
        }

        [Route("{id}")]
        public IActionResult GetById(int id)
        {
            var cat = db.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (cat == null)
            {
                return HttpNotFound();
            }
            return new ObjectResult(cat);
        }

    }
}
