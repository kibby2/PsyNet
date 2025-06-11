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

        public async Task<BlogPostComment> GetByIdAsync(Guid id)
        {
            return await psyNetDbContext.BlogPostComment.FindAsync(id);
        }

        public async Task<BlogPostComment> DeleteAsync(Guid id)
        {
            var comment = await psyNetDbContext.BlogPostComment.FindAsync(id);
            if (comment != null)
            {
                psyNetDbContext.BlogPostComment.Remove(comment);
                await psyNetDbContext.SaveChangesAsync();
            }
            return comment;
        }
    }
}
