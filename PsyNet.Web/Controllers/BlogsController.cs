using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IBlogPostLikeRepository blogPostLikeRepository; private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository; public BlogsController(IBlogPostRepository blogPostRepository,
            IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            // Clear any validation state from redirects
            ModelState.Clear();

            var liked = false;
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogDetailsViewModel();

            if (blogPost != null)
            {
                var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogPost.Id);

                if (signInManager.IsSignedIn(User))
                {
                    //Get like for this blog for this user
                    var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                    var userId = userManager.GetUserId(User);

                    if (userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        liked = likeFromUser != null;
                    }
                }
                //Get commenys for blog post
                var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id); var blogCommentsForView = new List<BlogComment>(); foreach (var blogComment in blogCommentsDomainModel)
                {
                    var commentUser = await userManager.FindByIdAsync(blogComment.UserId.ToString());
                    blogCommentsForView.Add(new BlogComment
                    {
                        Id = blogComment.Id,
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        Username = commentUser?.UserName ?? "Unknown User",
                        ProfilePictureUrl = commentUser?.ProfilePictureUrl
                    });
                }
                blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,
                    Comments = blogCommentsForView,
                    CommentDescription = string.Empty
                };
            }

            return View(blogDetailsViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if (signInManager.IsSignedIn(User))
            {
                if (!ModelState.IsValid)
                {
                    // If validation fails, reload the blog post data and return the view with errors
                    var blogPost = await blogPostRepository.GetByUrlHandleAsync(blogDetailsViewModel.UrlHandle);
                    if (blogPost != null)
                    {
                        var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogDetailsViewModel.Id);
                        var liked = false;                        // Get like status for this user
                        var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blogPost.Id);
                        var currentUserId = userManager.GetUserId(User);
                        if (currentUserId != null)
                        {
                            var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(currentUserId));
                            liked = likeFromUser != null;
                        }

                        var blogCommentsForView = new List<BlogComment>();
                        var blogComments = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);
                        foreach (var blogComment in blogComments)
                        {
                            var user = await userManager.FindByIdAsync(blogComment.UserId.ToString());
                            blogCommentsForView.Add(new BlogComment
                            {
                                Description = blogComment.Description,
                                DateAdded = blogComment.DateAdded,
                                Username = user?.UserName ?? "Unknown User",
                                ProfilePictureUrl = user?.ProfilePictureUrl,
                                Id = blogComment.Id
                            });
                        }

                        var blogDetailsViewModelWithData = new BlogDetailsViewModel
                        {
                            Id = blogPost.Id,
                            Content = blogPost.Content,
                            PageTitle = blogPost.PageTitle,
                            Author = blogPost.Author,
                            FeaturedImageUrl = blogPost.FeaturedImageUrl,
                            Heading = blogPost.Heading,
                            PublishedDate = blogPost.PublishedDate,
                            ShortDescription = blogPost.ShortDescription,
                            UrlHandle = blogPost.UrlHandle,
                            Visible = blogPost.Visible,
                            Tags = blogPost.Tags,
                            TotalLikes = totalLikes,
                            Liked = liked,
                            Comments = blogCommentsForView,
                            CommentDescription = blogDetailsViewModel.CommentDescription
                        };

                        return View(blogDetailsViewModelWithData);
                    }
                }

                var userId = userManager.GetUserId(User);
                if (userId != null)
                {
                    var domainModel = new BlogPostComment
                    {
                        BlogPostId = blogDetailsViewModel.Id,
                        Description = blogDetailsViewModel.CommentDescription,
                        UserId = Guid.Parse(userId),
                        DateAdded = DateTime.Now
                    };
                    await blogPostCommentRepository.AddAsync(domainModel);
                    return RedirectToAction("Index", "Blogs",
                        new { urlHandle = blogDetailsViewModel.UrlHandle });
                }
            }

            return View();
        }
    }
}
