using Microsoft.AspNetCore.Mvc;
using MyEducationPlan.Application.Services.Interfaces;

namespace MyEducationPlan.Controllers;

public class HomeController : Controller
{
    private readonly IProjectService _projectService;
    
    public HomeController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET: /Home
    [HttpGet]
    public async Task<IActionResult> GetProjects(string errorMessage)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            TempData["ErrorMessage"] = $"An error occurred: {errorMessage}";
        }
        
        var projects = await _projectService.GetProjects();
        
        return View(projects);
    }
    
    // NAV GET: /Home/ProjectFeedbacksList/{projectId}
    [HttpGet]
    public async Task<IActionResult> GetFeedbacksListByProjectId(int projectId)
    {
        try
        {
            var feedbacks = await _projectService.GetFeedbacksListByProjectId(projectId);
            
            return View(feedbacks);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            
            return RedirectToAction("GetProjects", "Home", new { errorMessage = ViewBag.ErrorMessage });
        }
    }
    
    //POST [CALCULATE AVERAGE RATING WITHIN SELECTED PROJECT]
    [HttpPost]
    public async Task<IActionResult> CalculateAverageRating(int projectId)
    {
        try
        {
            var average = await _projectService.CalculateAverageProjectRating(projectId);
            TempData["AverageRating"] = average.HasValue 
                ? $"Average Rating: {average:F2}" 
                : "No feedbacks available in project to calculate average.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }
        
        return RedirectToAction("GetFeedbacksListByProjectId", new { projectId });
    }
    
    //POST [DELETE ALL NEGATIVE FEEDBACKS IN SELECTED PROJECT]
    [HttpPost]
    public async Task<IActionResult> DeleteAllNegativeFeedbacks(int projectId)
    {
        try
        {
            await _projectService.DeleteAllNegativeFeedbacks(projectId);
            
            TempData["SuccessMessage"] = "All negative feedbacks deleted.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("GetFeedbacksListByProjectId", new { projectId });
    }
    
    //POST [DELETE SELECTED NEGATIVE FEEDBACK IN PROJECT]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSingleSelectedFeedback(int feedbackId, int projectId)
    {
        try
        {
            await _projectService.DeleteSingleFeedbackById(feedbackId);
            TempData["SuccessMessage"] = "Feedback deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        }

        return RedirectToAction("GetFeedbacksListByProjectId", new { projectId });
    }
}