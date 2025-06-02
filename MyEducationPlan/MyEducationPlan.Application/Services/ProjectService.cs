using MyEducationPlan.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEducationPlan.DataAccess;
using MyEducationPlan.Domain.Entities;

namespace MyEducationPlan.Application.Services;

public class ProjectService : IProjectService
{
    private readonly EducationPlanDbContext _dbContext;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(EducationPlanDbContext dbContext, ILogger<ProjectService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<InternProject> GetProjectById(int projectId)
    {
        var project = await _dbContext.InternProjects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
        {
            _logger.LogWarning("Intern Project with ID {ProjectId} not found.", projectId);
            throw new KeyNotFoundException($"Intern Project with ID {projectId} not found.");
        }
        
        return project;
    }
    
    public async Task<IEnumerable<InternProject>> GetProjects()
    {
        var projects = await _dbContext.InternProjects
            .ToListAsync();

        if (projects.Count == 0)
        {
            _logger.LogWarning("Intern Projects list not found or it's empty.");
            throw new KeyNotFoundException("Intern Projects List not found or it's empty.");
        }
        
        return projects;
    }

    public async Task<IEnumerable<InternProjectFeedback>> GetFeedbacksListByProjectId(int projectId)
    {
        
        var project = await _dbContext.InternProjects
            .Where(p => p.ProjectId == projectId)
            .Include(p => p.Feedbacks)
            .FirstOrDefaultAsync();
        
        if (project == null)
        {
            _logger.LogWarning("Intern Project with ID {ProjectId} not found.", projectId);
            throw new KeyNotFoundException($"Intern Project with ID {projectId} not found.");
        }

        if (!project.Feedbacks.Any())
        {
            _logger.LogWarning("No feedbacks found for project ID {ProjectId}.", projectId);
            return Enumerable.Empty<InternProjectFeedback>();
        }
        
        return project.Feedbacks;
    }
}