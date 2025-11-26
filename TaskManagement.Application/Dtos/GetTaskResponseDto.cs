using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


public class GetTaskResponseDto
{
    [Required]
    public int Id { get; set; }
    [Required, MinLength(1), MaxLength(50)]
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public TaskStatus Status { get; set; }

}

