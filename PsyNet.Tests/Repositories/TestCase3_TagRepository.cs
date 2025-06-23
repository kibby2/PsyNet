using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Repositories;
using System;
using System.Threading.Tasks;

namespace PsyNet.Tests.Repositories
{
    [TestFixture]
    public class TestCase3_TagRepository
    {
        private PsyNetDbContext _dbContext;
        private TagRepository _tagRepository;

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
            _tagRepository = new TagRepository(_dbContext);
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Dispose();
        }

        /// <summary>
        /// Test Case 3: TagRepository - Updating Tags
        /// 
        /// This test verifies that the TagRepository correctly updates 
        /// an existing tag's properties and saves the changes to the database.
        /// </summary>
        [Test]
        public async Task UpdateAsync_WithValidTag_ShouldUpdateTagInDatabase()
        {
            // STEP 1: Create and add a test tag
            var tagId = Guid.NewGuid();
            var originalTag = new Tag
            {
                Id = tagId,
                Name = "original-name",
                DisplayName = "Original Name"
            };

            await _dbContext.Tags.AddAsync(originalTag);
            await _dbContext.SaveChangesAsync();

            // STEP 2: Create an updated version of the tag
            var updatedTag = new Tag
            {
                Id = tagId,
                Name = "updated-name",
                DisplayName = "Updated Name"
            };

            // STEP 3: Call the repository method to update the tag
            var result = await _tagRepository.UpdateAsync(updatedTag);

            // STEP 4: Verify the update was successful
            Assert.That(result, Is.Not.Null, "UpdateAsync should return the updated tag");
            Assert.That(result.Name, Is.EqualTo("updated-name"), "Tag name should be updated");
            Assert.That(result.DisplayName, Is.EqualTo("Updated Name"), "Tag display name should be updated");

            // STEP 5: Verify the update was persisted to the database
            var tagFromDb = await _dbContext.Tags.FindAsync(tagId);
            Assert.That(tagFromDb, Is.Not.Null, "Tag should exist in database");
            Assert.That(tagFromDb.Name, Is.EqualTo("updated-name"), "Database tag name should be updated");
            Assert.That(tagFromDb.DisplayName, Is.EqualTo("Updated Name"), "Database tag display name should be updated");
        }

        /// <summary>
        /// Tests that the repository returns null when attempting to update
        /// a tag that doesn't exist in the database.
        /// </summary>
        [Test]
        public async Task UpdateAsync_WithNonExistentTag_ShouldReturnNull()
        {
            // STEP 1: Create a tag that doesn't exist in the database
            var nonExistentTag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = "non-existent",
                DisplayName = "Non Existent"
            };

            // STEP 2: Call the repository method to update the non-existent tag
            var result = await _tagRepository.UpdateAsync(nonExistentTag);

            // STEP 3: Verify null is returned
            Assert.That(result, Is.Null, "UpdateAsync should return null for non-existent tag");
        }
    }
}
