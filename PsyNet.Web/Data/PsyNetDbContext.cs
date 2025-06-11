using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Data
{
    public class PsyNetDbContext : DbContext
    {
        public PsyNetDbContext(DbContextOptions<PsyNetDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<BlogPostLike> BlogPostLike { get; set; }

        public DbSet<BlogPostComment> BlogPostComment { get; set; }

        public DbSet<BlogReport> BlogReports { get; set; }

        public DbSet<CommentReport> CommentReports { get; set; }
    }
}

