public interface ITaskService
{
    Task<GetTaskResponseDto> GetTaskByIdAsync(int taskId);
    Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync();
    Task<GetTaskResponseDto> AddTaskAsync(CreateTaskDto newTask);
    Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updatedTask);
    Task<bool> DeleteTaskAsync(int taskId);
    

}

