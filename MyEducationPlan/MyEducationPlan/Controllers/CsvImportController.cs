using Microsoft.AspNetCore.Mvc;
using MyEducationPlan.Application.Services.Interfaces;

namespace MyEducationPlan.Controllers;

public class CsvImportController : Controller
{
    private readonly ICsvService _csvService;

    public CsvImportController(ICsvService csvService)
    {
        _csvService = csvService;
    }
    
    // GET: View
    [HttpGet]
    public IActionResult UploadCsvFile()
    {
        return View();
    }

    // Post [UPLOAD AND IMPORT CSV FILE]
    [HttpPost]
    public async Task<IActionResult> UploadCsvFile(IFormFile csvFile)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid CSV file.";
            return View();
        }

        try
        {
            // Create temp "CsvUploads" folder and delete it after operation
            var uploadsFolder = Path.Combine(Path.GetTempPath(), "CsvUploads");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, Path.GetRandomFileName());

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await csvFile.CopyToAsync(stream);
            }

            await _csvService.ImportFeedbacksFromCsvFile(filePath);
            
            System.IO.File.Delete(filePath);

            TempData["SuccessMessage"] = "CSV import successful!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred during import: " + ex.Message;
        }

        return View();
    }
}