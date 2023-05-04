using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserWebApi.DatabaseModels;
using UserWebApi.Options;

namespace UserWebApi.Contexts;

public class UserContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserGroup> UsersGroup { get; set; } = null!;
    public DbSet<UserState> UsersState { get; set; } = null!;

    private readonly string _connection;

    public UserContext(IOptions<DatabaseSettings> settings)
    {
        _connection = settings.Value.DatabaseConnection;
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connection);
    }
}