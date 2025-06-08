using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public interface IBlogReportRepository
    {
        Task<BlogReport> AddAsync(BlogReport blogReport);
        Task<IEnumerable<BlogReport>> GetAllAsync(bool includeResolved = false);
        Task<bool> ExistsAsync(Guid blogPostId, string userId);
        Task<BlogReport> ResolveAsync(Guid reportId);
        Task<BlogReport> GetByIdAsync(Guid reportId);
    }
}