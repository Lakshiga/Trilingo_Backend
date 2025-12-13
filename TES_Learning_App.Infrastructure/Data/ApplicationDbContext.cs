using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // --- User & Profile World ---
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Admin> Admins { get; set; }

    // --- Content & Curriculum World ---
    public DbSet<Language> Languages { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Stage> Stages { get; set; }
    public DbSet<MainActivity> MainActivities { get; set; }
    public DbSet<ActivityType> ActivityTypes { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    // --- The Bridge ---
    public DbSet<StudentProgress> StudentProgresses { get; set; }

    // --- Payments ---
    public DbSet<LevelPurchase> LevelPurchases { get; set; }

    // This is the method that finds and applies all your configuration classes
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This line automatically scans the entire Infrastructure project for all
        // classes that implement IEntityTypeConfiguration and applies them.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

