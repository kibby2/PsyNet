using Microsoft.AspNetCore.Mvc;

namespace PsyNet.Web.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
