﻿namespace ProjectFeedbackModule.Domain.Entities;

public class InternProject
{
    public int ProjectId { get; set; } // Primary Key
    
    public string Name { get; set; }
    
    public string Owner { get; set; }
    
    public int EstimatedBudget { get; set; }

    public List<InternProjectFeedback> Feedbacks { get; set; } = new();
}