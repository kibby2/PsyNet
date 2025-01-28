using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly PsyNetDbContext psyNetDbContext;

        public BlogPostRepository(PsyNetDbContext psyNetDbContext)
        {
            this.psyNetDbContext = psyNetDbContext;
        }
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await psyNetDbContext.AddAsync(blogPost);
            await psyNetDbContext.SaveChangesAsync();
            return blogPost;
        }

        public Task<BlogPost?> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await psyNetDbContext.BlogPosts.Include(x=>x.Tags).ToListAsync();
        }
        

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await psyNetDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            throw new NotImplementedException();
        }
    }
}
