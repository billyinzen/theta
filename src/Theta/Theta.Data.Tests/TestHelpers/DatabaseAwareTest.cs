using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Theta.Data.Context;

namespace Theta.Data.Tests.TestHelpers;

public abstract class DatabaseAwareTest
{
    protected readonly ThetaDbContext TestDbContext;
    
    protected DatabaseAwareTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<ThetaDbContext>()
            .UseSqlite(connection)
            .Options;

        TestDbContext = new ThetaDbContext(contextOptions);
        TestDbContext.Database.EnsureCreated();
    }
}