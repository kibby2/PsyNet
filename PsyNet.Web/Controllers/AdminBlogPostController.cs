using Microsoft.AspNetCore.Mvc;

namespace PsyNet.Web.Controllers
{
    public class AdminBlogPostController : Controller
    {

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }
}
