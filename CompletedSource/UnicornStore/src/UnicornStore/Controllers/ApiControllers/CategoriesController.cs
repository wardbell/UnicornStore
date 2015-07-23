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
    public class CategoriesController : Controller
    {
        private IUnicornStoreContext db;

        public CategoriesController(IUnicornStoreContext dbContext)
        {
            db = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> GetAllCategories() 
        {
            return await db.Categories.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (cat == null)
            {
                return HttpNotFound();
            }
            return new ObjectResult(cat);
        }

    }
}
