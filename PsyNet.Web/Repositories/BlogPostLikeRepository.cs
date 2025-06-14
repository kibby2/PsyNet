using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly PsyNetDbContext psyNetDbContext;

        public BlogPostLikeRepository(PsyNetDbContext psyNetDbContext)
        {
            this.psyNetDbContext = psyNetDbContext;
        }
        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            await psyNetDbContext.BlogPostLike.AddAsync(blogPostLike);
            await psyNetDbContext.SaveChangesAsync();
            return blogPostLike;
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            return await psyNetDbContext.BlogPostLike.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            return await psyNetDbContext.BlogPostLike
                .CountAsync(x => x.BlogPostId == blogPostId);
        }

        public async Task<BlogPostLike?> RemoveLikeForBlog(Guid blogPostId, Guid userId)
        {
            var like = await psyNetDbContext.BlogPostLike
                .FirstOrDefaultAsync(x => x.BlogPostId == blogPostId && x.UserId == userId);

            if (like != null)
            {
                psyNetDbContext.BlogPostLike.Remove(like);
                await psyNetDbContext.SaveChangesAsync();
                return like;
            }

            return null;
        }
    }
}
