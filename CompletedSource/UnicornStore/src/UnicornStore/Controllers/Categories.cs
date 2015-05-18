using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.AspNet.Controllers
{
    //Todo: move all Entity Framework interaction to a repository/unit-of-work

    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private IUnicornStoreContext db;

        public CategoriesController(IUnicornStoreContext dbContext)
        {
            db = dbContext;
        }

        [HttpGet]
        public IEnumerable<Category> GetAllCategories() 
        {
            return db.Categories;
        }

        [HttpGet("{id}")]
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
