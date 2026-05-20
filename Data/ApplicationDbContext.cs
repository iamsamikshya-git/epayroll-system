using Microsoft.EntityFrameworkCore;
using E_PayRoll.Models;

namespace E_PayRoll.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Municipality> Municipalities { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<TeacherLevel> TeacherLevels { get; set; }
    public DbSet<TeacherCategory> TeacherCategories { get; set; }
    public DbSet<BasicSalary> BasicSalaries { get; set; }
    public DbSet<AppointmentType> AppointmentTypes { get; set; }
    public DbSet<GradeNumber> GradeNumbers { get; set; }
    public DbSet<Salary> Salaries { get; set; }
    public DbSet<Notification> Notifications { get; set; }



}