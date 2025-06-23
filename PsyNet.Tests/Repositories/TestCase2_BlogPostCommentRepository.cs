using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsyNet.Tests.Repositories
{
    [TestFixture]
    public class TestCase2_BlogPostCommentRepository
    {
        private PsyNetDbContext _dbContext;
        private BlogPostCommentRepository _commentRepository;
        private Guid _blogPostId;
        private List<BlogPostComment> _testComments;

        [SetUp]
        public async Task Setup()
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

            // Setup test data
            await SetupTestDataAsync();
        }

        private async Task SetupTestDataAsync()
        {
            // STEP 1: Create a test blog post
            _blogPostId = Guid.NewGuid();

            var blogPost = new BlogPost
            {
                Id = _blogPostId,
                Heading = "Test Blog Post",
                PageTitle = "Test Page Title",
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

            // STEP 2: Create multiple test comments for the blog post
            _testComments = new List<BlogPostComment>
            {
                new BlogPostComment
                {
                    Description = "First test comment",
                    BlogPostId = _blogPostId,
                    UserId = Guid.NewGuid(),
                    DateAdded = DateTime.UtcNow.AddMinutes(-30)
                },
                new BlogPostComment
                {
                    Description = "Second test comment",
                    BlogPostId = _blogPostId,
                    UserId = Guid.NewGuid(),
                    DateAdded = DateTime.UtcNow.AddMinutes(-20)
                },
                new BlogPostComment
                {
                    Description = "Third test comment",
                    BlogPostId = _blogPostId,
                    UserId = Guid.NewGuid(),
                    DateAdded = DateTime.UtcNow.AddMinutes(-10)
                }
            };

            // Add comments to a different blog post to ensure filtering works
            var differentBlogPostId = Guid.NewGuid();
            var differentBlogPost = new BlogPost
            {
                Id = differentBlogPostId,
                Heading = "Different Blog Post",
                PageTitle = "Different Page Title",
                Content = "Different Content",
                ShortDescription = "Different Description",
                FeaturedImageUrl = "different-image.jpg",
                UrlHandle = "different-blog",
                PublishedDate = DateTime.UtcNow,
                Author = "Different Author",
                Visible = true
            };

            _dbContext.BlogPosts.Add(differentBlogPost);

            var differentComment = new BlogPostComment
            {
                Description = "Comment on a different blog post",
                BlogPostId = differentBlogPostId,
                UserId = Guid.NewGuid(),
                DateAdded = DateTime.UtcNow
            };

            // Add all comments to the database
            _dbContext.BlogPostComment.AddRange(_testComments);
            _dbContext.BlogPostComment.Add(differentComment);

            await _dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        /// <summary>
        /// Test Case 2: BlogPostCommentRepository - Retrieving Comments By Blog ID
        /// 
        /// This test verifies that the BlogPostCommentRepository correctly retrieves
        /// all comments associated with a specific blog post ID.
        /// </summary>
        [Test]
        public async Task GetCommentsByBlogIdAsync_ShouldReturnOnlyCommentsForSpecificBlog()
        {
            // STEP 1: Call the repository method to get comments for our blog post
            var retrievedComments = await _commentRepository.GetCommentsByBlogIdAsync(_blogPostId);

            // STEP 2: Verify correct comments were returned
            Assert.That(retrievedComments, Is.Not.Null, "Retrieved comments should not be null");

            // Convert to list for easier assertions
            var commentsList = retrievedComments.ToList();

            // STEP 3: Verify correct count of comments
            Assert.That(commentsList.Count, Is.EqualTo(_testComments.Count),
                "Should return the exact number of comments for this blog post");

            // STEP 4: Verify all comments are for the correct blog post
            foreach (var comment in commentsList)
            {
                Assert.That(comment.BlogPostId, Is.EqualTo(_blogPostId),
                    "All comments should be for the requested blog post");
            }

            // STEP 5: Verify all expected comments were returned
            var descriptions = commentsList.Select(c => c.Description).ToList();
            foreach (var testComment in _testComments)
            {
                Assert.That(descriptions, Contains.Item(testComment.Description),
                    $"Comment with description '{testComment.Description}' should be returned");
            }
        }

        /// <summary>
        /// Tests that the repository returns an empty collection when 
        /// requesting comments for a non-existent blog post.
        /// </summary>
        [Test]
        public async Task GetCommentsByBlogIdAsync_WithNonExistentBlog_ShouldReturnEmptyCollection()
        {
            // STEP 1: Generate a random blog ID that doesn't exist
            var nonExistentBlogId = Guid.NewGuid();

            // STEP 2: Call the repository method
            var retrievedComments = await _commentRepository.GetCommentsByBlogIdAsync(nonExistentBlogId);

            // STEP 3: Verify an empty collection is returned, not null
            Assert.That(retrievedComments, Is.Not.Null, "Should return empty collection, not null");
            Assert.That(retrievedComments, Is.Empty, "Should return empty collection for non-existent blog");
        }
    }
}
