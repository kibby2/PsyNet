using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Models.Domain;

namespace PsyNet.Web.Data
{
    public class PsyNetDbContext : DbContext
    {
        public PsyNetDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}

