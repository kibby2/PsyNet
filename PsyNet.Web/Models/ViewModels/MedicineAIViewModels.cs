using System.ComponentModel.DataAnnotations;

namespace PsyNet.Web.Models.ViewModels
{
    public class AddPatientViewModel
    {
        [Required]
        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public string Sex { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Medical Condition")]
        public string Condition { get; set; } = string.Empty;

        [Display(Name = "Patient Name (Optional)")]
        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }
    }

    public class PatientDetailsViewModel
    {
        public Guid Id { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string? PatientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MedicineRecommendationViewModel> Recommendations { get; set; } = new();
    }

    public class MedicineRecommendationViewModel
    {
        public Guid Id { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Instructions { get; set; }
        public string? RecommendationSource { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class SavedPatientsViewModel
    {
        public List<PatientSummaryViewModel> Patients { get; set; } = new();
    }

    public class PatientSummaryViewModel
    {
        public Guid Id { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string? PatientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RecommendationCount { get; set; }
    }
}
