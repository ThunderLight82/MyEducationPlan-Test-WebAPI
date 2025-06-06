namespace ProjectFeedbackModule.Application.Services.Interfaces;

public interface ICsvService
{ 
    Task ImportFeedbacksFromCsvFile(string filePath);
}