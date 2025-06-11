using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class CommentReportRepository : ICommentReportRepository
    {
        private readonly PsyNetDbContext psyNetDbContext;

        public CommentReportRepository(PsyNetDbContext psyNetDbContext)
        {
            this.psyNetDbContext = psyNetDbContext;
        }

        public async Task<CommentReport> AddAsync(CommentReport commentReport)
        {
            await psyNetDbContext.CommentReports.AddAsync(commentReport);
            await psyNetDbContext.SaveChangesAsync();
            return commentReport;
        }

        public async Task<IEnumerable<CommentReport>> GetAllAsync(bool includeResolved = false)
        {
            return await psyNetDbContext.CommentReports
                .Where(x => includeResolved || !x.Resolved)
                .OrderByDescending(x => x.ReportDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid commentId, string userId)
        {
            return await psyNetDbContext.CommentReports
                .AnyAsync(x => x.CommentId == commentId && x.ReporterUserId == userId && !x.Resolved);
        }

        public async Task<CommentReport> ResolveAsync(Guid reportId)
        {
            var report = await psyNetDbContext.CommentReports.FindAsync(reportId);
            if (report != null)
            {
                report.Resolved = true;
                await psyNetDbContext.SaveChangesAsync();
            }
            return report;
        }

        public async Task<CommentReport> GetByIdAsync(Guid reportId)
        {
            return await psyNetDbContext.CommentReports.FindAsync(reportId);
        }
    }
}