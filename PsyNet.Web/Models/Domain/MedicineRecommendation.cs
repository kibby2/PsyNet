using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.Domain
{
    public class MedicineRecommendation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; } // Foreign key to Patient        [Required]
        [StringLength(200)]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; } // e.g., "500mg", "2 tablets"

        [StringLength(100)]
        public string? Frequency { get; set; } // e.g., "Twice daily", "Once daily"

        [StringLength(1000)]
        public string? Instructions { get; set; } // Additional instructions

        [Range(0, 10)]
        public double ConfidenceScore { get; set; } // AI confidence score (0-10)

        [StringLength(50)]
        public string? RecommendationSource { get; set; } // "AI", "Manual", etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true; // To mark if recommendation is still valid        // Navigation property
        public Patient? Patient { get; set; }
    }
}
