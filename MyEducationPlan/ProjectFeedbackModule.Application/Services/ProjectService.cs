using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectFeedbackModule.Application.Services.Interfaces;
using ProjectFeedbackModule.DataAccess;
using ProjectFeedbackModule.Domain;
using ProjectFeedbackModule.Domain.Entities;

namespace ProjectFeedbackModule.Application.Services;

public class ProjectService : IProjectService
{
    private readonly EducationPlanDbContext _dbContext;
    private readonly FeedbackSettings _settings;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(EducationPlanDbContext dbContext, ILogger<ProjectService> logger, IOptions<FeedbackSettings> settings)
    {
        _dbContext = dbContext;
        _logger = logger;
        _settings = settings.Value;
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
            _logger.LogWarning("Intern Project with ID [{ProjectId}] not found.", projectId);
            throw new KeyNotFoundException($"Intern Project with ID [{projectId}] not found.");
        }

        if (!project.Feedbacks.Any())
        {
            _logger.LogWarning("No feedbacks found for project ID [{ProjectId}].", projectId);
            return Enumerable.Empty<InternProjectFeedback>();
        }
        
        return project.Feedbacks;
    }
    
    public async Task<double?> CalculateAverageProjectRating(int projectId)
    {
        var project = await _dbContext.InternProjects
            .Include(p => p.Feedbacks)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
        {
            _logger.LogWarning("Project with ID [{ProjectId}] not found.", projectId);
            throw new KeyNotFoundException($"Project with ID [{projectId}] not found.");
        }

        if (!project.Feedbacks.Any())
        {
            _logger.LogWarning("No feedbacks found for project ID [{ProjectId}].", projectId);
            return null;
        }

        return project.Feedbacks.Average(f => f.Rating);
    }
    
    public async Task DeleteAllNegativeFeedbacks(int projectId)
    {
        var negativeRatingThreshold = _settings.NegativeRatingThreshold;
        
        var project = await _dbContext.InternProjects
            .Include(p => p.Feedbacks)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
        {
            _logger.LogWarning("Project with ID [{ProjectId}] not found.", projectId);
            throw new KeyNotFoundException($"Project with ID [{projectId}] not found.");
        }

        var negativeFeedbacks = project.Feedbacks
            .Where(f => f.Rating < negativeRatingThreshold)
            .ToList();

        if (!negativeFeedbacks.Any())
        {
            _logger.LogWarning("No negative feedbacks found for project ID [{ProjectId}].", projectId);
            return;
        }

        _dbContext.InternProjectFeedbacks.RemoveRange(negativeFeedbacks);
        
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("{Count} negative feedbacks removed from project ID [{ProjectId}].", negativeFeedbacks.Count, projectId);
    }
    
    public async Task DeleteSingleFeedbackById(int feedbackId)
    {
        var feedback = await _dbContext.InternProjectFeedbacks
            .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

        if (feedback == null)
        {
            _logger.LogWarning("Feedback with ID [{FeedbackId}] not found.", feedbackId);
            throw new KeyNotFoundException($"Feedback with ID [{feedbackId}] not found.");
        }

        _dbContext.InternProjectFeedbacks.Remove(feedback);
        
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Feedback with ID [{FeedbackId}] successfully deleted.", feedbackId);
    }
}