using Microsoft.EntityFrameworkCore;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }
    public async Task AddTaskAsync (TaskEntity newTask) => await _context.Tasks.AddAsync(newTask);

    public async Task DeleteTaskAsync(TaskEntity taskToDelete) => _context.Tasks.Remove(taskToDelete);

    public async Task UpdateTaskAsync(TaskEntity taskToUpdate) => _context.Tasks.Update(taskToUpdate);

    public async Task<List<TaskEntity>> GetAllTasksAsync(string userId) => await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();

    public async Task<TaskEntity?> GetTaskByIdAsync(int taskId, string userId) => await _context.Tasks.Where(t => t.Id == taskId && t.UserId == userId).FirstOrDefaultAsync();

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();


}

