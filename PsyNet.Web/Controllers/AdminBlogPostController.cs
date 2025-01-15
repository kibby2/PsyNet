using Microsoft.AspNetCore.Mvc;

namespace PsyNet.Web.Controllers
{
    public class AdminBlogPostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
