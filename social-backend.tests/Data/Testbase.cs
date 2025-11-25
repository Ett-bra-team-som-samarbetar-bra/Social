using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using social_backend;

namespace social_backend.tests.Data
{
    public abstract class TestBase : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly SocialContext Context;

        protected TestBase()
        {
            // Single in-memory SQLite connection per test instance
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<SocialContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            Context = new SocialContext(options);

            // Build schema for this in-memory DB
            Context.Database.EnsureCreated();

            // Optional seed
            SeedData();
        }

        /// <summary>
        /// Override in test classes to insert seed data.
        /// </summary>
        protected virtual void SeedData()
        {
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }
}
