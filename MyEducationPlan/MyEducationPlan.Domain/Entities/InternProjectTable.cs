namespace MyEducationPlan.Domain.Entities;

public class InternProjectTable
{
    public int ProjectId { get; set; } // Primary Key
    
    public string Name { get; set; }
    
    public string Owner { get; set; }
    
    public int EstimatedBudget { get; set; }
}