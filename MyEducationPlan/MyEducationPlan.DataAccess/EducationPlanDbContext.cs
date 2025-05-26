using Microsoft.EntityFrameworkCore;
using MyEducationPlan.Domain.Entities;

namespace MyEducationPlan.DataAccess;

public class EducationPlanDbContext : DbContext
{
    public DbSet<InternProject> InternProjects { get; set; } = null!;
    public DbSet<InternProjectFeedback> InternProjectFeedbacks { get; set; } = null!;
    
    public EducationPlanDbContext(DbContextOptions<EducationPlanDbContext> options) : base(options) { }
    
    // Some design parameters for entities and prepopulate ProjectTable inside DB list for quick personal testing
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InternProject>()
            .HasIndex(g => g.Name)
            .IsUnique();

        modelBuilder.Entity<InternProject>().HasData
        (
            new() 
                { ProjectId = 1, Name = "Online Store", Owner = "Vitaliy Kovalchuk", EstimatedBudget = 1800 },
            new()
                { ProjectId = 2, Name = "Find Missing People - Volunteer project", Owner = "Olexander Borozuy",
                EstimatedBudget = 1000 }
        );
        
        modelBuilder.Entity<InternProjectFeedback>().HasData
        (
            new() 
                { FeedbackId = 1, ProjectId = 1, EmployeeName = "Daniela Kilova", Rating = 8 , Comment = "Тест фітбек №1"},
            new()
            { FeedbackId = 2, ProjectId = 1, EmployeeName = "Ethan Douglass", Rating = 6 , Comment = "Тест фітбек №2"},
            
            new() 
            { FeedbackId = 1, ProjectId = 2, EmployeeName = "Olexander Borozuy", Rating = 10, Comment = "Тест фітбек волонтерка №1"}
        );
    }
}