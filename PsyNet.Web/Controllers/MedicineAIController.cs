using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Repositories;
using PsyNet.Web.Services;

namespace PsyNet.Web.Controllers
{
    [Authorize]
    public class MedicineAIController : Controller
    {
        private readonly IPatientRepository patientRepository;
        private readonly IMedicineRecommendationRepository medicineRecommendationRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMedicineRecommendationService medicineRecommendationService;

        public MedicineAIController(
            IPatientRepository patientRepository,
            IMedicineRecommendationRepository medicineRecommendationRepository,
            UserManager<IdentityUser> userManager,
            IMedicineRecommendationService medicineRecommendationService)
        {
            this.patientRepository = patientRepository;
            this.medicineRecommendationRepository = medicineRecommendationRepository;
            this.userManager = userManager;
            this.medicineRecommendationService = medicineRecommendationService;
        }

        [HttpGet]
        public IActionResult Recommend()
        {
            var viewModel = new AddPatientViewModel();
            return View(viewModel);
        }        [HttpPost]
        public async Task<IActionResult> Recommend(AddPatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = userManager.GetUserId(User);
                if (userId == null)
                {
                    TempData["NotificationMessage"] = "User not found. Please log in again.";
                    TempData["NotificationType"] = "error";
                    return RedirectToAction("Login", "Account");
                }

                // Create patient record
                var patient = new Models.Domain.Patient
                {
                    UserId = userId,
                    Age = model.Age,
                    Sex = model.Sex,
                    Condition = model.Condition,
                    PatientName = model.PatientName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var savedPatient = await patientRepository.AddAsync(patient);

                // Get AI recommendations using the real service
                var ageGroup = MapAgeToGroup(model.Age);
                var aiRecommendations = await medicineRecommendationService.GetRecommendationsAsync(
                    ageGroup, model.Sex, model.Condition, savedPatient.Id);

                // Save recommendations to database
                foreach (var recommendation in aiRecommendations)
                {
                    await medicineRecommendationRepository.AddAsync(recommendation);
                }

                TempData["NotificationMessage"] = $"Patient information saved successfully! {aiRecommendations.Count} AI recommendations generated.";
                TempData["NotificationType"] = "success";

                return RedirectToAction("PatientDetails", new { id = savedPatient.Id });
            }
            catch (Exception)
            {
                TempData["NotificationMessage"] = "An error occurred while saving patient information. Please try again.";
                TempData["NotificationType"] = "error";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SavedPatients()
        {
            try
            {
                var userId = userManager.GetUserId(User);
                if (userId == null)
                {
                    TempData["NotificationMessage"] = "User not found. Please log in again.";
                    TempData["NotificationType"] = "error";
                    return RedirectToAction("Login", "Account");
                }

                var patients = await patientRepository.GetPatientsByUserIdAsync(userId);
                
                var viewModel = new SavedPatientsViewModel
                {
                    Patients = patients.Select(p => new PatientSummaryViewModel
                    {
                        Id = p.Id,
                        Age = p.Age,
                        Sex = p.Sex,
                        Condition = p.Condition,
                        PatientName = p.PatientName,
                        CreatedAt = p.CreatedAt,
                        RecommendationCount = p.Recommendations.Count
                    }).ToList()
                };

                return View(viewModel);
            }            catch (Exception)
            {
                TempData["NotificationMessage"] = "An error occurred while loading saved patients.";
                TempData["NotificationType"] = "error";
                return View(new SavedPatientsViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails(Guid id)
        {
            try
            {
                var userId = userManager.GetUserId(User);
                var patient = await patientRepository.GetByIdAsync(id);

                if (patient == null || patient.UserId != userId)
                {
                    TempData["NotificationMessage"] = "Patient not found or access denied.";
                    TempData["NotificationType"] = "error";
                    return RedirectToAction("SavedPatients");
                }

                var viewModel = new PatientDetailsViewModel
                {
                    Id = patient.Id,
                    Age = patient.Age,
                    Sex = patient.Sex,
                    Condition = patient.Condition,
                    PatientName = patient.PatientName,
                    CreatedAt = patient.CreatedAt,
                    UpdatedAt = patient.UpdatedAt,
                    Recommendations = patient.Recommendations.Select(r => new MedicineRecommendationViewModel
                    {
                        Id = r.Id,
                        MedicineName = r.MedicineName,
                        Dosage = r.Dosage,
                        Frequency = r.Frequency,
                        Instructions = r.Instructions,
                        ConfidenceScore = r.ConfidenceScore,
                        RecommendationSource = r.RecommendationSource,
                        CreatedAt = r.CreatedAt,
                        IsActive = r.IsActive
                    }).OrderByDescending(r => r.CreatedAt).ToList()
                };

                return View(viewModel);
            }            catch (Exception)
            {
                TempData["NotificationMessage"] = "An error occurred while loading patient details.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("SavedPatients");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            try
            {
                var userId = userManager.GetUserId(User);
                var patient = await patientRepository.GetByIdAsync(id);

                if (patient == null || patient.UserId != userId)
                {
                    TempData["NotificationMessage"] = "Patient not found or access denied.";
                    TempData["NotificationType"] = "error";
                    return RedirectToAction("SavedPatients");
                }

                var deleted = await patientRepository.DeleteAsync(id);
                
                if (deleted)
                {
                    TempData["NotificationMessage"] = "Patient record deleted successfully.";
                    TempData["NotificationType"] = "success";
                }
                else
                {
                    TempData["NotificationMessage"] = "Failed to delete patient record.";
                    TempData["NotificationType"] = "error";
                }

                return RedirectToAction("SavedPatients");
            }
            catch (Exception)
            {
                TempData["NotificationMessage"] = "An error occurred while deleting the patient record.";
                TempData["NotificationType"] = "error";
                return RedirectToAction("SavedPatients");
            }
        }

        private string MapAgeToGroup(int age)
        {
            return age switch
            {
                <= 24 => "18-24",
                <= 34 => "25-34", 
                <= 44 => "35-44",
                <= 54 => "45-54",
                <= 64 => "55-64",
                <= 74 => "65-74",
                _ => "75+"
            };
        }
    }
}
