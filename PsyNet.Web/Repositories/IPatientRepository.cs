using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient> AddAsync(Patient patient);
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IEnumerable<Patient>> GetPatientsByUserIdAsync(string userId);        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
