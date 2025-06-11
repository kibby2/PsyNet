using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public interface ICommentReportRepository
    {
        Task<CommentReport> AddAsync(CommentReport commentReport);
        Task<IEnumerable<CommentReport>> GetAllAsync(bool includeResolved = false);
        Task<bool> ExistsAsync(Guid commentId, string userId);
        Task<CommentReport> ResolveAsync(Guid reportId);
        Task<CommentReport> GetByIdAsync(Guid reportId);
    }
}