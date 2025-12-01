using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : IdentityDbContext(options)
{
    public DbSet<TaskEntity> Tasks { get; set; }
}
