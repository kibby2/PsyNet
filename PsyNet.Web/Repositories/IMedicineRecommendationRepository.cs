using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public interface IMedicineRecommendationRepository
    {
        Task<MedicineRecommendation> AddAsync(MedicineRecommendation recommendation);
        Task<MedicineRecommendation?> GetByIdAsync(Guid id);
        Task<IEnumerable<MedicineRecommendation>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<MedicineRecommendation>> GetActiveRecommendationsByPatientIdAsync(Guid patientId);
        Task<MedicineRecommendation> UpdateAsync(MedicineRecommendation recommendation);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeactivateAsync(Guid id);
    }
}
