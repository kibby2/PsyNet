using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.ViewModels
{
    public class AddTagRequest
    {
        [Required(ErrorMessage = "Tag Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Tag Name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Tag Name can only contain letters, numbers, hyphens, and underscores (no spaces)")]
        public string TagName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Display Name must be between 2 and 100 characters")]
        public string DisplayName { get; set; } = string.Empty;
    }
}
