using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


public class TaskEntity
{
    [Key]
    public int Id { get; set; }
    [Required, MinLength(1), MaxLength(50)]
    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TaskStatus Status { get; set; }
    
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }  
}

