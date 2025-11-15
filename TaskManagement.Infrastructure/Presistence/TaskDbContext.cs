using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

internal class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    internal DbSet<Task> Tasks { get; set; }
}
