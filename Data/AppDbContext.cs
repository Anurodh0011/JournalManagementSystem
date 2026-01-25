using JournalManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JournalManagementSystem.Data;
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Journal> Journals { get; set; } = null!;

    private readonly string _dbPath;

    public AppDbContext()
    {
        // Path to store SQLite DB on device
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Combine(folder, "app.db");
        Debug.WriteLine($"Database path: {_dbPath}");
        Console.WriteLine($"Database path: {_dbPath}");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}