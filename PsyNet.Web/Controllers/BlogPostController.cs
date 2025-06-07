using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Repositories;
using PsyNet.Web.Models.Domain;


namespace PsyNet.Web.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogPostController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository, UserManager<IdentityUser> userManager)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Add()
        {
            // Get tags for the dropdown
            var tags = tagRepository.GetAllAsync().Result;

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }),
                // Auto-set today's date
                PublishedDate = DateTime.Today,
                // Set the author to current user
                Author = User.Identity.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            // Set current date & author
            addBlogPostRequest.PublishedDate = DateTime.Today;
            addBlogPostRequest.Author = User.Identity.Name;

            // Map view model to domain model
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible
            };

            // Map Tags from selected tags
            var selectedTags = new List<Tag>();
            foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
            }
            //Mapping tags back to domain model
            blogPost.Tags = selectedTags;

            await blogPostRepository.AddAsync(blogPost);

            // Add success notification
            TempData["NotificationMessage"] = "Blog post added successfully!";
            TempData["NotificationType"] = "success";

            // Redirect to home page
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            // Call the repository 
            var blogPosts = await blogPostRepository.GetAllAsync();

            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagsDomainModel = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {
                // map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };

                return View(model);

            }


            return View(null);
        }

        // Edit action (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            // Map view model to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                PublishedDate = editBlogPostRequest.PublishedDate,
                Author = editBlogPostRequest.Author,
                Visible = editBlogPostRequest.Visible,
            };
            // Map tags into domain model

            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            // Submit information to repository to update
            var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if (updatedBlog != null)
            {
                // Add success notification
                TempData["NotificationMessage"] = "Blog post updated successfully!";
                TempData["NotificationType"] = "success";

                return RedirectToAction("Index", "Home");
            }

            // Show error notification
            return RedirectToAction("Edit");
        }

        [HttpPost]

        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            // Talk to repository to delete this blog post and tags
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                // Add success notification
                TempData["NotificationMessage"] = "Blog post deleted successfully!";
                TempData["NotificationType"] = "warning";

                return RedirectToAction("Index", "Home");
            }

            // Show error notification
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }
}
