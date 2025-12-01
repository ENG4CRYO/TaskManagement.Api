public interface ITaskService
{
    Task<GetTaskResponseDto> GetTaskByIdAsync(int taskId);
    Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync(string userId);
    Task<GetTaskResponseDto> AddTaskAsync(CreateTaskDto newTask, string userId);
    Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updatedTask);
    Task<bool> DeleteTaskAsync(int taskId);
    

}

