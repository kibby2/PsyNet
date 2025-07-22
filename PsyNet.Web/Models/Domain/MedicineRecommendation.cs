using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.Domain
{
    public class MedicineRecommendation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        [StringLength(200)]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Dosage { get; set; }

        [StringLength(100)]
        public string? Frequency { get; set; }

        [StringLength(1000)]
        public string? Instructions { get; set; }

        [Range(0, 10)]
        public double ConfidenceScore { get; set; }

        [StringLength(50)]
        public string? RecommendationSource { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
        public Patient? Patient { get; set; }
    }
}
