using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
