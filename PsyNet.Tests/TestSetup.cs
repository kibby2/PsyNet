using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using System;

namespace PsyNet.Tests
{
    /// <summary>
    /// Test helper class for setting up in-memory database contexts for unit testing
    /// </summary>
    public static class TestSetup
    {
        /// <summary>
        /// Creates an in-memory PsyNetDbContext for testing purposes
        /// </summary>
        /// <returns>An isolated in-memory database context</returns>
        public static PsyNetDbContext GetTestDbContext()
        {
            // Create unique database name for test isolation
            var dbName = $"PsyNetTestDb_{Guid.NewGuid()}";

            // Configure DbContext options to use in-memory database
            var options = new DbContextOptionsBuilder<PsyNetDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            // Create and return a new context instance
            return new PsyNetDbContext(options);
        }
    }
}
