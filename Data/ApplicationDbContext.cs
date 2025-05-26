using Microsoft.EntityFrameworkCore;
using E_PayRoll.Models;

namespace E_PayRoll.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<School> Schools { get; set; }
}