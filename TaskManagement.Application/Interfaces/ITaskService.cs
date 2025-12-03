public interface ITaskService
{
    Task<GetTaskResponseDto> GetTaskByIdAsync(int taskId,string userId);
    Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync(string userId);
    Task<GetTaskResponseDto> AddTaskAsync(CreateTaskDto newTask, string userId);
    Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updatedTask,string userId);
    Task<bool> DeleteTaskAsync(int taskId,string userId);
    

}

