using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly PsyNetDbContext psyNetDbContext;

        public BlogPostCommentRepository(PsyNetDbContext psyNetDbContext)
        {
            this.psyNetDbContext = psyNetDbContext;
        }
        public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
        {
            await psyNetDbContext.BlogPostComment.AddAsync(blogPostComment);
            await psyNetDbContext.SaveChangesAsync();
            return blogPostComment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetCommentsByBlogIdAsync(Guid blogPostId)
        {
            return await psyNetDbContext.BlogPostComment
                .Where(x => x.BlogPostId == blogPostId)
                .ToListAsync();
        }
    }
}
