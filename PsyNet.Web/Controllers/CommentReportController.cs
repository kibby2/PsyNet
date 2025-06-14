using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Repositories;

namespace PsyNet.Web.Controllers
{
    public class CommentReportController : Controller
    {
        private readonly ICommentReportRepository commentReportRepository;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;
        private readonly IBlogPostRepository blogPostRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CommentReportController(
            ICommentReportRepository commentReportRepository,
            IBlogPostCommentRepository blogPostCommentRepository,
            IBlogPostRepository blogPostRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.commentReportRepository = commentReportRepository;
            this.blogPostCommentRepository = blogPostCommentRepository;
            this.blogPostRepository = blogPostRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Report(Guid commentId, Guid blogPostId)
        {
            // Get the comment
            var comments = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPostId);
            var comment = comments.FirstOrDefault(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Get the blog post
            var blogPost = await blogPostRepository.GetAsync(blogPostId);
            if (blogPost == null)
            {
                return NotFound();
            }

            // Get comment author
            var commentAuthor = await userManager.FindByIdAsync(comment.UserId.ToString());

            // Check if user is trying to report their own comment
            if (User.Identity.IsAuthenticated && commentAuthor?.UserName == User.Identity.Name)
            {
                TempData["NotificationMessage"] = "You cannot report your own comment.";
                TempData["NotificationType"] = "warning";
                return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
            }

            var viewModel = new ReportCommentViewModel
            {
                CommentId = commentId,
                BlogPostId = blogPostId,
                BlogPostTitle = blogPost.Heading,
                CommentContent = comment.Description,
                CommentAuthor = commentAuthor?.UserName ?? "Unknown"
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitReport(ReportCommentViewModel model)
        {
            // Get the comment 
            var comments = await blogPostCommentRepository.GetCommentsByBlogIdAsync(model.BlogPostId);
            var comment = comments.FirstOrDefault(c => c.Id == model.CommentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Get the blog post
            var blogPost = await blogPostRepository.GetAsync(model.BlogPostId);
            if (blogPost == null)
            {
                return NotFound();
            }

            var userId = userManager.GetUserId(User);

            // Check if a report for this comment by this user already exists
            var reportExists = await commentReportRepository.ExistsAsync(model.CommentId, userId);
            if (reportExists)
            {
                TempData["NotificationMessage"] = "You have already reported this comment.";
                TempData["NotificationType"] = "info";
                return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
            }

            // Create the report
            var report = new CommentReport
            {
                CommentId = model.CommentId,
                ReporterUserId = userId,
                Comment = model.Comment ?? "No comment provided",
                ReportDate = DateTime.Now,
                Resolved = false
            };

            await commentReportRepository.AddAsync(report);

            TempData["NotificationMessage"] = "Comment reported successfully. An admin will review it.";
            TempData["NotificationType"] = "success";

            return RedirectToAction("Index", "Blogs", new { urlHandle = blogPost.UrlHandle });
        }
        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> List()
        {
            var reports = await commentReportRepository.GetAllAsync();
            var viewModel = new CommentReportsViewModel
            {
                Reports = await MapReportsToViewModel(reports)
            };

            return View(viewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Resolve(Guid reportId)
        {
            await commentReportRepository.ResolveAsync(reportId);

            TempData["NotificationMessage"] = "Comment report resolved successfully.";
            TempData["NotificationType"] = "success";

            return RedirectToAction("List");
        }
        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteComment(Guid reportId)
        {
            var report = await commentReportRepository.GetByIdAsync(reportId);
            if (report != null)
            {
                // Delete the comment
                await blogPostCommentRepository.DeleteAsync(report.CommentId);

                // Resolve the report
                await commentReportRepository.ResolveAsync(reportId);

                TempData["NotificationMessage"] = "Comment deleted and report resolved.";
                TempData["NotificationType"] = "warning";
            }

            return RedirectToAction("List");
        }

        private async Task<IEnumerable<CommentReportItem>> MapReportsToViewModel(IEnumerable<CommentReport> reports)
        {
            var reportItems = new List<CommentReportItem>();

            foreach (var report in reports)
            {
                var comment = await blogPostCommentRepository.GetByIdAsync(report.CommentId);
                if (comment == null) continue;

                var blogPost = await blogPostRepository.GetAsync(comment.BlogPostId);
                if (blogPost == null) continue;

                var reporter = await userManager.FindByIdAsync(report.ReporterUserId);
                var commentAuthor = await userManager.FindByIdAsync(comment.UserId.ToString());

                reportItems.Add(new CommentReportItem
                {
                    ReportId = report.Id,
                    CommentId = report.CommentId,
                    BlogPostId = comment.BlogPostId,
                    BlogPostTitle = blogPost.Heading,
                    BlogUrlHandle = blogPost.UrlHandle,
                    CommentContent = comment.Description,
                    CommentAuthor = commentAuthor?.UserName ?? "Unknown",
                    ReporterName = reporter?.UserName ?? "Unknown",
                    ReportComment = report.Comment,
                    ReportDate = report.ReportDate
                });
            }

            return reportItems;
        }
    }
}