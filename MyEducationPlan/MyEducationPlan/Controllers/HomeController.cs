
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
            ViewBag.ErrorMessage = errorMessage;
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
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return RedirectToAction("GetProjects", "Home", new { errorMessage = ViewBag.ErrorMessage });
        }
    }
}