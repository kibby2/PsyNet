using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Services
{
    public interface IMedicineRecommendationService
    {
        Task<List<MedicineRecommendation>> GetRecommendationsAsync(string age, string sex, string condition, Guid patientId);
        List<string> GetAvailableAgeGroups();
        List<string> GetAvailableConditions();
    }
}
