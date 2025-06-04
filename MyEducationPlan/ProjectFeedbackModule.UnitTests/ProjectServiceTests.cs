/*using Microsoft.EntityFrameworkCore;
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
    private readonly Mock<ILogger<ProjectService>> _mockLoggerService;
    private readonly EducationPlanDbContext _dbContext;
    private readonly EducationPlanDbContext _emptyDbContext;
    private readonly IProjectService _projectService;
    private readonly FeedbackSettings _settings;
    
    public ProjectServiceTests()
    {
        _settings = new IOptions<FeedbackSettings>;
        _mockLoggerService = new Mock<ILogger<ProjectService>>();

        _dbContext = CreateAndSeedTestDb();
        
        _projectService = new ProjectService(_dbContext, _mockLoggerService.Object,);
        
        //Separate empty DB for null or empty cases.  
        _emptyDbContext = CreateEmptyTestDb(); 
    }
    
    [Fact]
    public async Task GetProjectsAsList_ProjectsExist_ReturnCorrectProjectsAsListFromRepo()
    {
        //Arrange & Act
        var projectsListResult = await _projectService.GetProjects();

        //Assert
        Assert.NotNull(projectsListResult);
        Assert.Equal(3, projectsListResult.Count());
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
        var projectService = new ProjectService(_dbContext, _mockLoggerService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await projectService.GetProjectById(default));
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await projectService.GetProjectById(0));
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
        Assert.Contains(feedbacks, f => f.Rating == 0);
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
        var projectService = new ProjectService(_emptyDbContext, _mockLoggerService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => projectService.GetFeedbacksListByProjectId(projectId));
    }
    
    
    // Moq DB and seed it with any Test data.
    private EducationPlanDbContext CreateAndSeedTestDb()
    {
        var options = new DbContextOptionsBuilder<EducationPlanDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_EducationPlanDB")
            .Options;
        
        // Add seed data to the in-memory database for tests cases. 
        var dbContext = new EducationPlanDbContext(options);
        
        dbContext.InternProjects.AddRange(
            new() { ProjectId = 1, Name = "Test Proj 1", Owner = "Test Owner 1", EstimatedBudget = 999 },
            new() { ProjectId = 2, Name = "Test Proj 2", Owner = "Test Owner 2", EstimatedBudget = 0 },
            new() { ProjectId = 3, Name = "Test Proj 3 Empty", Owner = "Test Owner 3", EstimatedBudget = 100 }
        );
        
        dbContext.InternProjectFeedbacks.AddRange(
            new() { FeedbackId = 1, Comment = "Test Comment 1", Rating = 9, ProjectId = 1 },
            new() { FeedbackId = 2, Comment = "Test Comment 2", Rating = 0, ProjectId = 1 },
            new() { FeedbackId = 3, Comment = "Test Comment 3", Rating = 10, ProjectId = 2 }
        );

        dbContext.SaveChangesAsync();

        return dbContext;
    }
    
    private EducationPlanDbContext CreateEmptyTestDb()
    {
        var options = new DbContextOptionsBuilder<EducationPlanDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_EmptyEducationPlanDB")
            .Options;

        return new EducationPlanDbContext(options);
    }
}*/