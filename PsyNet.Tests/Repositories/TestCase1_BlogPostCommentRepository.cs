using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PsyNet.Tests.Repositories
{
    [TestFixture]
    public class TestCase1_BlogPostCommentRepository
    {
        private PsyNetDbContext _dbContext;
        private BlogPostCommentRepository _commentRepository;

        [SetUp]
        public void Setup()
        {
            // Create a test database name
            var dbName = $"TestDb_{Guid.NewGuid()}";

            // Create DB context options using in-memory database
            var options = new DbContextOptionsBuilder<PsyNetDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            // Create database context
            _dbContext = new PsyNetDbContext(options);

            // Create repository with test database
            _commentRepository = new BlogPostCommentRepository(_dbContext);
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        /// <summary>
        /// Test Case 1: BlogPostCommentRepository - Adding Comments
        /// 
        /// This test verifies that the BlogPostCommentRepository correctly adds 
        /// new comments to the database and associates them with the correct blog post and user.
        /// </summary>
        [Test]
        public async Task AddAsync_WithValidComment_ShouldAddToDatabase()
        {            // STEP 1: Create a test blog post
            var userId = Guid.NewGuid();  // User ID as Guid to match BlogPostComment model
            var blogPostId = Guid.NewGuid();
            var blogPost = new BlogPost
            {
                Id = blogPostId,
                Heading = "Test Blog Post",
                PageTitle = "Test Page Title", // Adding the missing required property
                Content = "Test Content for the blog post",
                ShortDescription = "Test Description",
                FeaturedImageUrl = "test-image.jpg",
                UrlHandle = "test-blog",
                PublishedDate = DateTime.UtcNow,
                Author = "Test Author",
                Visible = true
            };

            _dbContext.BlogPosts.Add(blogPost);
            await _dbContext.SaveChangesAsync();

            // STEP 2: Create a new comment
            var comment = new BlogPostComment
            {
                Description = "This is a test comment.",
                BlogPostId = blogPostId,
                UserId = userId,
                DateAdded = DateTime.UtcNow
            };

            // STEP 3: Add the comment using the repository method
            var addedComment = await _commentRepository.AddAsync(comment);

            // STEP 4: Verify the comment was added with correct properties
            Assert.That(addedComment, Is.Not.Null, "Comment should not be null after adding");
            Assert.That(addedComment.Id, Is.Not.EqualTo(Guid.Empty), "Comment should have a valid ID");

            // STEP 5: Verify the comment exists in the database
            var commentFromDb = await _dbContext.BlogPostComment.FindAsync(addedComment.Id);
            Assert.That(commentFromDb, Is.Not.Null, "Comment should exist in database");
            Assert.That(commentFromDb.Description, Is.EqualTo("This is a test comment."), "Comment description should match");
            Assert.That(commentFromDb.UserId, Is.EqualTo(userId), "Comment user ID should match");
            Assert.That(commentFromDb.BlogPostId, Is.EqualTo(blogPostId), "Comment blog post ID should match");
        }
    }
}
