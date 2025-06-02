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

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlog = await psyNetDbContext.BlogPosts.FindAsync(id);

            if (existingBlog != null)
            {
                psyNetDbContext.BlogPosts.Remove(existingBlog);
                await psyNetDbContext.SaveChangesAsync();
                return existingBlog;
            }

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await psyNetDbContext.BlogPosts.Include(x => x.Tags).ToListAsync();
        }


        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await psyNetDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await psyNetDbContext.BlogPosts.Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlog = await psyNetDbContext.BlogPosts.Include(x => x.Tags)
                 .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlog != null)
            {
                existingBlog.Id = blogPost.Id;
                existingBlog.Heading = blogPost.Heading;
                existingBlog.PageTitle = blogPost.PageTitle;
                existingBlog.Content = blogPost.Content;
                existingBlog.ShortDescription = blogPost.ShortDescription;
                existingBlog.Author = blogPost.Author;
                existingBlog.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlog.UrlHandle = blogPost.UrlHandle;
                existingBlog.Visible = blogPost.Visible;
                existingBlog.PublishedDate = blogPost.PublishedDate;
                existingBlog.Tags = blogPost.Tags;

                await psyNetDbContext.SaveChangesAsync();
                return existingBlog;
            }

            return null;
        }
    }
}
