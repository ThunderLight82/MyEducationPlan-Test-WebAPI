using MyEducationPlan.Domain.Entities;

namespace MyEducationPlan.Application.Services.Interfaces;

public interface ICsvService
{ 
    Task ImportFeedbacksFromCsvFileAsync(string filePath);
}