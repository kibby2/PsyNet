using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.ViewModels
{
    public class AddTagRequest
    {
        [Required]
        public string TagName { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
