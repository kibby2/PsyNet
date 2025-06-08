using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Repositories;

namespace PsyNet.Web.Controllers
{
    public class BlogReportController : Controller
    {
        private readonly IBlogReportRepository blogReportRepository;
        private readonly IBlogPostRepository blogPostRepository;
        private readonly UserManager<IdentityUser> userManager;

        public BlogReportController(IBlogReportRepository blogReportRepository,
                                    IBlogPostRepository blogPostRepository,
                                    UserManager<IdentityUser> userManager)
        {
            this.blogReportRepository = blogReportRepository;
            this.blogPostRepository = blogPostRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Report(Guid blogId)
        {
            var blogPost = await blogPostRepository.GetAsync(blogId);
            if (blogPost == null)
            {
                return NotFound();
            }

            // Check if user is trying to report their own blog
            if (User.Identity.Name == blogPost.Author)
            {
                TempData["NotificationMessage"] = "You cannot report your own blog post.";
                TempData["NotificationType"] = "warning";
                return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
            }

            var viewModel = new ReportBlogViewModel
            {
                BlogPostId = blogId,
                BlogPostTitle = blogPost.Heading
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitReport(ReportBlogViewModel model)
        {
            var blogPost = await blogPostRepository.GetAsync(model.BlogPostId);
            if (blogPost == null)
            {
                return NotFound();
            }

            var userId = userManager.GetUserId(User);

            // Check if a report for this blog by this user already exists
            var reportExists = await blogReportRepository.ExistsAsync(model.BlogPostId, userId);
            if (reportExists)
            {
                TempData["NotificationMessage"] = "You have already reported this blog post.";
                TempData["NotificationType"] = "info";
                return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
            }

            // Create the report 
            var report = new BlogReport
            {
                BlogPostId = model.BlogPostId, 
                ReporterUserId = userId,
                Comment = model.Comment ?? "No comment provided",
                ReportDate = DateTime.Now,
                Resolved = false
                
            };

            await blogReportRepository.AddAsync(report);

            TempData["NotificationMessage"] = "Blog post reported successfully. An admin will review it.";
            TempData["NotificationType"] = "success";

            return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var reports = await blogReportRepository.GetAllAsync();
            var viewModel = new BlogReportsViewModel
            {
                Reports = await MapReportsToViewModel(reports)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Resolve(Guid reportId)
        {
            await blogReportRepository.ResolveAsync(reportId);

            TempData["NotificationMessage"] = "Report resolved successfully.";
            TempData["NotificationType"] = "success";

            return RedirectToAction("List");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBlog(Guid reportId)
        {
            var report = await blogReportRepository.GetByIdAsync(reportId);
            if (report != null)
            {
                // Delete the blog post
                await blogPostRepository.DeleteAsync(report.BlogPostId);

                // Resolve the report
                await blogReportRepository.ResolveAsync(reportId);

                TempData["NotificationMessage"] = "Blog post deleted and report resolved.";
                TempData["NotificationType"] = "warning";
            }

            return RedirectToAction("List");
        }

        private async Task<IEnumerable<BlogReportItem>> MapReportsToViewModel(IEnumerable<BlogReport> reports)
        {
            var reportItems = new List<BlogReportItem>();

            foreach (var report in reports)
            {
                var reporter = await userManager.FindByIdAsync(report.ReporterUserId);

                reportItems.Add(new BlogReportItem
                {
                    ReportId = report.Id,
                    BlogPostId = report.BlogPostId,
                    BlogTitle = report.BlogPost?.Heading ?? "Unknown",
                    BlogAuthor = report.BlogPost?.Author ?? "Unknown",
                    ReporterName = reporter?.UserName ?? "Unknown",
                    ReportComment = report.Comment,
                    ReportDate = report.ReportDate,
                    BlogUrlHandle = report.BlogPost?.UrlHandle
                });
            }

            return reportItems;
        }
    }
}