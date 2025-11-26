using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


public class UpdateTaskDto
{
    [Required, MinLength(1), MaxLength(50)]
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public TaskStatus Status { get; set; }
}

