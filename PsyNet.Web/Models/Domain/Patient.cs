using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.Domain
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }        [Required]
        public string UserId { get; set; } = string.Empty; // Foreign key to AspNetUsers

        [Required]
        [Range(0, 150)]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Sex { get; set; } = string.Empty; // "Male", "Female", "Other"

        [Required]
        [StringLength(500)]
        public string Condition { get; set; } = string.Empty; // Medical condition description

        [StringLength(100)]
        public string? PatientName { get; set; } // Optional patient name for reference

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for recommendations
        public ICollection<MedicineRecommendation> Recommendations { get; set; } = new List<MedicineRecommendation>();
    }
}
