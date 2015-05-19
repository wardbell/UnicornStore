using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
namespace UnicornStore.AspNet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageProductsSpaController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Angular spa goes here.";

            return View();
        }
    }
}
