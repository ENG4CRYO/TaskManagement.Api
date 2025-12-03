public interface ITaskRepository
{
    Task AddTaskAsync(TaskEntity newTask);

    Task UpdateTaskAsync(TaskEntity updatedTask);

    Task DeleteTaskAsync(TaskEntity taskToDelete);

    Task<TaskEntity?> GetTaskByIdAsync(int TaskId,string userId);
    Task<List<TaskEntity>> GetAllTasksAsync(string userId);

    Task SaveChangesAsync( );
}

