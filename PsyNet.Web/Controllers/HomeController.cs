using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PsyNet.Web.Models;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Repositories;

namespace PsyNet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ITagRepository tagRepository;

        public HomeController(ILogger<HomeController> logger, IBlogPostRepository blogPostRepository, ITagRepository tagRepository)
        {
            _logger = logger;
            this.blogPostRepository = blogPostRepository;
            this.tagRepository = tagRepository;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 3; // 3 blogs per page

            // getting all blogs
            var allBlogPosts = await blogPostRepository.GetAllAsync();

            // Calculate pagination
            var totalBlogs = allBlogPosts.Count();
            var totalPages = (int)Math.Ceiling((double)totalBlogs / pageSize);

            // Ensure page is within valid range
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Get blogs for current page
            var blogPosts = allBlogPosts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // get all tags
            var tags = await tagRepository.GetAllAsync();

            var model = new HomeViewModel
            {
                BlogPosts = blogPosts,
                Tags = tags,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalBlogs = totalBlogs,
                PageSize = pageSize
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
