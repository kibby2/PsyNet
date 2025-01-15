using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Models.ViewModels;

namespace PsyNet.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly PsyNetDbContext psyNetDbContext;
        public TagRepository(PsyNetDbContext psyNetDbContext)
        {
            this.psyNetDbContext = psyNetDbContext;
        }

        public PsyNetDbContext PsyNetDbContext { get; }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await psyNetDbContext.Tags.AddAsync(tag);
            await psyNetDbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingTag = await psyNetDbContext.Tags.FindAsync(id);

            if (existingTag != null)
            {
                psyNetDbContext.Tags.Remove(existingTag);
                await psyNetDbContext.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await psyNetDbContext.Tags.ToListAsync();
        }

        public Task<Tag?> GetAsync(Guid id)
        {
            return psyNetDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existingTag = await psyNetDbContext.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await psyNetDbContext.SaveChangesAsync();

                return existingTag;
            }
            return null;
        }
    }
}
