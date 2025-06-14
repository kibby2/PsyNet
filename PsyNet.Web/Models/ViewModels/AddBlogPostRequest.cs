using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.ViewModels
{
    public class AddBlogPostRequest
    {
        [Required(ErrorMessage = "Heading is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Heading must be between 5 and 200 characters")]
        public string Heading { get; set; } = string.Empty;

        [Required(ErrorMessage = "Page Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Page Title must be between 5 and 200 characters")]
        public string PageTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [MinLength(50, ErrorMessage = "Content must be at least 50 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Short Description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Short Description must be between 10 and 500 characters")]
        public string ShortDescription { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? FeaturedImageUrl { get; set; }

        [Required(ErrorMessage = "URL Handle is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "URL Handle must be between 5 and 100 characters")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "URL Handle can only contain lowercase letters, numbers, and hyphens")]
        public string? UrlHandle { get; set; }

        [Required(ErrorMessage = "Published Date is required")]
        public DateTime PublishedDate { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters")]
        public string Author { get; set; } = string.Empty;



        // Display tags
        public IEnumerable<SelectListItem> Tags { get; set; } = new List<SelectListItem>();

        // Collect Tag
        public string[] SelectedTags { get; set; } = Array.Empty<string>();
    }
}
