using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class MedicineRecommendationRepository : IMedicineRecommendationRepository
    {
        private readonly PsyNetDbContext context;

        public MedicineRecommendationRepository(PsyNetDbContext context)
        {
            this.context = context;
        }

        public async Task<MedicineRecommendation> AddAsync(MedicineRecommendation recommendation)
        {
            await context.MedicineRecommendations.AddAsync(recommendation);
            await context.SaveChangesAsync();
            return recommendation;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var recommendation = await context.MedicineRecommendations.FindAsync(id);
            if (recommendation != null)
            {
                recommendation.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var recommendation = await context.MedicineRecommendations.FindAsync(id);
            if (recommendation != null)
            {
                context.MedicineRecommendations.Remove(recommendation);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<MedicineRecommendation>> GetActiveRecommendationsByPatientIdAsync(Guid patientId)
        {
            return await context.MedicineRecommendations
                .Where(r => r.PatientId == patientId && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicineRecommendation>> GetByPatientIdAsync(Guid patientId)
        {
            return await context.MedicineRecommendations
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<MedicineRecommendation?> GetByIdAsync(Guid id)
        {
            return await context.MedicineRecommendations
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<MedicineRecommendation> UpdateAsync(MedicineRecommendation recommendation)
        {
            context.MedicineRecommendations.Update(recommendation);
            await context.SaveChangesAsync();
            return recommendation;
        }
    }
}
