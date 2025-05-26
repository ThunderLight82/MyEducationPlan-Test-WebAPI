namespace MyEducationPlan.Domain.Entities;

public class InternProjectFeedback
{
    public int FeedbackId { get; set; } // Primary Key
    
    public int ProjectId { get; set; } // Foreign Key > InternProjectTable.ProjectId
    
    public string EmployeeName { get; set; }
    
    public int Rating { get; set; }
    
    public string Comment { get; set; }
}