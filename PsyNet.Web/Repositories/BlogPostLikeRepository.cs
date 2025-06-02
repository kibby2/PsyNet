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

        public Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            return await psyNetDbContext.BlogPostLike
                .CountAsync(x => x.BlogPostId == blogPostId);
        }
    }
}
