using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PsyNetDbContext context;

        public PatientRepository(PsyNetDbContext context)
        {
            this.context = context;
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            await context.Patients.AddAsync(patient);
            await context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient != null)
            {
                context.Patients.Remove(patient);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await context.Patients.AnyAsync(p => p.Id == id);
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await context.Patients
                .Include(p => p.Recommendations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByUserIdAsync(string userId)
        {
            return await context.Patients
                .Include(p => p.Recommendations)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            patient.UpdatedAt = DateTime.UtcNow;
            context.Patients.Update(patient);
            await context.SaveChangesAsync();
            return patient;
        }
    }
}
