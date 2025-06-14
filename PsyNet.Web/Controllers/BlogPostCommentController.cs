using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Repositories;

namespace PsyNet.Web.Controllers
{
    [Authorize]
    public class BlogPostCommentController : Controller
    {
        private readonly IBlogPostCommentRepository blogPostCommentRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public BlogPostCommentController(IBlogPostCommentRepository blogPostCommentRepository, UserManager<ApplicationUser> userManager)
        {
            this.blogPostCommentRepository = blogPostCommentRepository;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid commentId, string urlHandle)
        {
            var comment = await blogPostCommentRepository.GetByIdAsync(commentId);

            if (comment != null)
            {
                var currentUserId = userManager.GetUserId(User);

                // Only allow the comment author to delete their own comment
                if (currentUserId != null && comment.UserId == Guid.Parse(currentUserId))
                {
                    await blogPostCommentRepository.DeleteAsync(commentId);
                    TempData["Success"] = "Comment deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "You can only delete your own comments.";
                }
            }
            else
            {
                TempData["Error"] = "Comment not found.";
            }

            return RedirectToAction("Index", "Blogs", new { urlHandle = urlHandle });
        }
    }
}
