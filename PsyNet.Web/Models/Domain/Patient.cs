using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.Domain
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [Range(0, 90)]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Sex { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Condition { get; set; } = string.Empty;
        [StringLength(100)]
        public string? PatientName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for recommendations
        public ICollection<MedicineRecommendation> Recommendations { get; set; } = new List<MedicineRecommendation>();
    }
}
