using MyEducationPlan.Domain.Entities;

namespace MyEducationPlan.Application.Services.Interfaces;

public interface IProjectService
{
    Task<InternProject> GetProjectById(int projectId);
    Task<IEnumerable<InternProject>> GetProjects();
    Task<IEnumerable<InternProjectFeedback>> GetFeedbacksListByProjectId(int projectId);
    Task<double> CalculateAverageProjectRating(int projectId);
    Task DeleteAllNegativeFeedbacks(int projectId);
    Task DeleteSingleFeedbackById(int feedbackId);
}