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

        public DbSet<Patient> Patients { get; set; }

        public DbSet<MedicineRecommendation> MedicineRecommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Patient entity relationships
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Recommendations)
                .WithOne(r => r.Patient)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure CommentReport relationship (existing)
            modelBuilder.Entity<CommentReport>()
                .HasIndex(cr => new { cr.CommentId, cr.ReporterUserId })
                .IsUnique(); // Prevent duplicate reports from same user for same comment
        }
    }
}

