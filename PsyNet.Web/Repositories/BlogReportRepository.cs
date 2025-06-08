using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class BlogReportRepository : IBlogReportRepository
    {
        private readonly PsyNetDbContext psyNetDbContext;

        public BlogReportRepository(PsyNetDbContext psyNetDbContext)
        {
            this.psyNetDbContext = psyNetDbContext;
        }

        public async Task<BlogReport> AddAsync(BlogReport blogReport)
        {
            await psyNetDbContext.BlogReports.AddAsync(blogReport);
            await psyNetDbContext.SaveChangesAsync();
            return blogReport;
        }

        public async Task<IEnumerable<BlogReport>> GetAllAsync(bool includeResolved = false)
        {
            return await psyNetDbContext.BlogReports
                .Include(x => x.BlogPost)
                .Where(x => includeResolved || !x.Resolved)
                .OrderByDescending(x => x.ReportDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid blogPostId, string userId)
        {
            return await psyNetDbContext.BlogReports
                .AnyAsync(x => x.BlogPostId == blogPostId && x.ReporterUserId == userId && !x.Resolved);
        }

        public async Task<BlogReport> ResolveAsync(Guid reportId)
        {
            var report = await psyNetDbContext.BlogReports.FindAsync(reportId);
            if (report != null)
            {
                report.Resolved = true;
                await psyNetDbContext.SaveChangesAsync();
            }
            return report;
        }

        public async Task<BlogReport> GetByIdAsync(Guid reportId)
        {
            return await psyNetDbContext.BlogReports
                .Include(x => x.BlogPost)
                .FirstOrDefaultAsync(x => x.Id == reportId);
        }
    }
}