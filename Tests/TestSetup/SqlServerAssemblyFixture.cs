using Microsoft.Extensions.Configuration;

[assembly: AssemblyFixture(typeof(SqlServerAssemblyFixture))]

namespace Tests.TestSetup;

public class SqlServerAssemblyFixture : IDisposable
{
    private readonly AppDbContext _context;
    public readonly DbContextOptions<AppDbContext> Options;
    private bool _disposed;
    public SqlServerAssemblyFixture()
    {

        var config = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .Build();

        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(config["SqlServer:ConnectionString"])
            .Options;
        _context = new AppDbContext(Options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

