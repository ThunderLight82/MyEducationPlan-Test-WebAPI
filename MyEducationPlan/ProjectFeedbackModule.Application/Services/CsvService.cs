using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectFeedbackModule.Application.DTOs;
using ProjectFeedbackModule.Application.Services.Interfaces;
using ProjectFeedbackModule.DataAccess;
using ProjectFeedbackModule.Domain.Entities;

namespace ProjectFeedbackModule.Application.Services;

public class CsvService : ICsvService
{
    private readonly EducationPlanDbContext _dbContext;
    private readonly ILogger<CsvService> _logger;

    public CsvService(EducationPlanDbContext dbContext, ILogger<CsvService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task ImportFeedbacksFromCsvFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("CSV file not found.", filePath); 
        }
        
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<CsvFeedbackRecord>().ToList();

        foreach (var record in records)
        {
            var project = await _dbContext.InternProjects
                .FirstOrDefaultAsync(p => p.Name == record.ProjectName);

            // Proper project name validation. Project name should already exist in DB.
            if (project == null)
            {
                _logger.LogWarning("No project found with name: [{ProjectName}].", record.ProjectName);
                throw new KeyNotFoundException($"Error in file. No project found with name: [{record.ProjectName}]. Please, check project name.");
            }
            
            // Rating number validation. MAX = 10; MIN = 1 
            if (record.Rating is < 1 or > 10)
            {
                _logger.LogWarning("Invalid rating [{Rating}] in project [{ProjectName}].", record.Rating, record.ProjectName);
                throw new InvalidDataException($"Rating must be between 1 and 10. Invalid rating: [{record.Rating}] in project [{record.ProjectName}].");
            }

            var feedback = new InternProjectFeedback
            {
                EmployeeName = record.EmployeeName,
                Rating = record.Rating,
                Comment = record.Comment,
                ProjectId = project.ProjectId
            };

            _dbContext.InternProjectFeedbacks.Add(feedback);
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Feedbacks from CSV imported successfully.");
    }
}

