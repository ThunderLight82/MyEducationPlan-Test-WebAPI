using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectFeedbackModule.Application.Services.Interfaces;
using ProjectFeedbackModule.DataAccess;
using Moq;
using ProjectFeedbackModule.Application.Services;
using ProjectFeedbackModule.Domain;
using Xunit;

namespace ProjectFeedbackModule.UnitTests;

public class ProjectServiceTests
{
    private readonly Mock<ILogger<ProjectService>> _mockLoggerProjectService;
    private readonly ProjectModuleDbContext _dbContext;
    private readonly ProjectModuleDbContext _emptyDbContext;
    private readonly IProjectService _projectService;
    private readonly FeedbackSettings _settings;
    private readonly IOptions<FeedbackSettings> _options;
    
    public ProjectServiceTests()
    {
        _settings = new FeedbackSettings
        {
            NegativeRatingThreshold = 5
        };
        _options = Options.Create(_settings);
        
        _mockLoggerProjectService = new Mock<ILogger<ProjectService>>();

        _dbContext = CreateAndSeedTestDb();
        
        _projectService = new ProjectService(_dbContext,_mockLoggerProjectService.Object, _options);
  
        _emptyDbContext = CreateEmptyTestDb(); 
    }
    
    [Fact]
    public async Task GetProjectsAsList_ProjectsExist_ReturnCorrectProjectsAsListFromRepo()
    {
        //Arrange & Act
        var projectsListResult = await _projectService.GetProjects();

        //Assert
        Assert.NotNull(projectsListResult);
        Assert.Equal(4, projectsListResult.Count());
        Assert.Contains(projectsListResult, c => c.Name == "Test Proj 1");
        Assert.Contains(projectsListResult, c => c.Name == "Test Proj 2");
        Assert.Contains(projectsListResult, c => c.Name == "Test Proj 3 Empty");
    }

    [Fact]
    public async Task GetProjById_ProjExist_ReturnCorrectProjByIdFromRepo()
    {
        // Arrange
        var projectId = 2;

        // Act
        var projectIdResult = await _projectService.GetProjectById(projectId);
            
        // Assert
        Assert.NotNull(projectIdResult);
        Assert.Equal(projectId, projectIdResult.ProjectId);
        Assert.Equal("Test Proj 2", projectIdResult.Name);
    }
    
    [Fact]
    public async Task GetCourseById_NotDataFoundInDbOrNull_ThrowException()
    {
        // Arrange
        var projectServiceWithEmptyDb = new ProjectService(_emptyDbContext, _mockLoggerProjectService.Object, _options);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await projectServiceWithEmptyDb.GetProjectById(default));
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await projectServiceWithEmptyDb.GetProjectById(0));
    }
    
    [Fact]
    public async Task GetFeedbacksByProjId_FeedbacksAndProjExist_ReturnCorrectFeedbackListFromRepo()
    {
        // Arrange & Act
        var feedbacks = await _projectService.GetFeedbacksListByProjectId(2);

        // Assert
        Assert.NotNull(feedbacks);
        Assert.Single(feedbacks);
        Assert.Contains(feedbacks, f => f.Comment == "Test Comment 3");
    }
    
    [Fact]
    public async Task GetFeedbacksByProjId_FeedbacksAndProjExist_ReturnAnotherCorrectFeedbackListFromRepo()
    { 
        // Arrange & Act
        var feedbacks = await _projectService.GetFeedbacksListByProjectId(1);

        // Assert
        Assert.NotNull(feedbacks);
        Assert.Equal(2, feedbacks.Count());
        Assert.Contains(feedbacks, f => f.Rating == 9);  
        Assert.Contains(feedbacks, f => f.Rating == 1);
    }
    
    [Fact]
    public async Task GetFeedbacksByProjId_ProjExistButItsEmptyOnFeedbacks_ReturnEmptyList()
    { 
        // Arrange & Act
        var result = await _projectService.GetFeedbacksListByProjectId(3);
        
        // Assert
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(123)]
    [InlineData(-1)]
    public async Task GetFeedbacksByProjId_NotDataFoundInDbOrNull_ThrowException(int projectId)
    { 
        // Arrange
        var projectServiceWithEmptyDb = new ProjectService(_emptyDbContext, _mockLoggerProjectService.Object, _options);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => projectServiceWithEmptyDb.GetFeedbacksListByProjectId(projectId));
    }
    
    [Fact]
    public async Task CalculateAverageProjectRating_CalculateAverageRating_ReturnsCorrectAverageFromProj()
    {
        // Arrange
        var projectId = 1;

        // Act
        var result = await _projectService.CalculateAverageProjectRating(projectId);

        // Assert [(9 + 1) / 2]
        Assert.NotNull(result);
        Assert.Equal(5.0, result);
    }
    
    [Fact]
    public async Task CalculateAverageProjectRating_NonExistingId_ThrowException()
    {
        // Arrange
        var projectServiceWithEmptyDb = new ProjectService(_emptyDbContext, _mockLoggerProjectService.Object, _options);
        var nonExistentProjectId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            projectServiceWithEmptyDb.CalculateAverageProjectRating(nonExistentProjectId));
        Assert.Equal($"Project with ID [{nonExistentProjectId}] not found.", exception.Message);
    }
    
    [Fact]
    public async Task CalculateAverageProjectRating_NotDataFoundInDbOrNull_ThrowException()
    {
        // Arrange
        var projectIdWithNoFeedback = 3;

        // Act
        var result = await _projectService.CalculateAverageProjectRating(projectIdWithNoFeedback);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteSingleFeedbackById_ProjExistButNotProj_ThrowException()
    {
        // Arrange
        var projectId = 1;
        var nonExistentFeedbackId = 999;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _projectService.DeleteSingleFeedbackById(projectId, nonExistentFeedbackId));
        Assert.Equal($"Feedback with ID [{nonExistentFeedbackId}] not found in project ID [{projectId}].", exception.Message);
    }
    
    // Moq DB and seed it with any Test data.
    private ProjectModuleDbContext CreateAndSeedTestDb()
    {
        var options = new DbContextOptionsBuilder<ProjectModuleDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_ProjectModuleDb")
            .Options;
        
        // Add seed data to the in-memory database for tests cases. 
        var dbContext = new ProjectModuleDbContext(options);
        
        dbContext.InternProjects.AddRange(
            new() { ProjectId = 1, Name = "Test Proj 1", Owner = "Test Owner 1", EstimatedBudget = 999 },
            new() { ProjectId = 2, Name = "Test Proj 2", Owner = "Test Owner 2", EstimatedBudget = 0 },
            new() { ProjectId = 3, Name = "Test Proj 3 Empty", Owner = "Test Owner 3", EstimatedBudget = 100 },
            new() { ProjectId = 101, Name = "Test Proj 3 For Delete Test", Owner = "Test Owner 4", EstimatedBudget = 10 }
        );
        
        dbContext.InternProjectFeedbacks.AddRange(
            new() { FeedbackId = 1, Comment = "Test Comment 1", Rating = 9, ProjectId = 1 },
            new() { FeedbackId = 2, Comment = "Test Comment 2", Rating = 1, ProjectId = 1 },
            new() { FeedbackId = 3, Comment = "Test Comment 3", Rating = 10, ProjectId = 2 },
            new() { FeedbackId = 52, Comment = "Test Comment 3 To Delete", Rating = 3, ProjectId = 101 }
        );

        dbContext.SaveChangesAsync();

        return dbContext;
    }
    
    
    private ProjectModuleDbContext CreateEmptyTestDb()
    {
        var options = new DbContextOptionsBuilder<ProjectModuleDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_EmptyProjectModuleDbPlanDB")
            .Options;

        return new ProjectModuleDbContext(options);
    }
}