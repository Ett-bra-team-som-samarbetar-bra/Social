using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SocialBackend.Data;


namespace SocialBackend.tests.Data
{
    public abstract class TestBase : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly DatabaseContext Context;

        protected TestBase()
        {
            // Single in-memory SQLite connection per test instance
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            Context = new DatabaseContext(options);
            Context.Database.EnsureCreated();
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
