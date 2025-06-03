using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEducationPlan.Application.DTOs;
using MyEducationPlan.Application.Services.Interfaces;
using MyEducationPlan.DataAccess;
using MyEducationPlan.Domain.Entities;

namespace MyEducationPlan.Application.Services;

public class CsvService : ICsvService
{
    private readonly EducationPlanDbContext _dbContext;
    private readonly ILogger<CsvService> _logger;

    public CsvService(EducationPlanDbContext dbContext, ILogger<CsvService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task ImportFeedbacksFromCsvFileAsync(string filePath)
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

            if (project == null)
            {
                _logger.LogWarning("No project found with name: {ProjectName}", record.ProjectName);
                throw new KeyNotFoundException($"Error in file. No project found with name: {record.ProjectName}. Please, check project name.");
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

