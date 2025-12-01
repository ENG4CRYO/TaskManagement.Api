using AutoMapper;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    public async Task<GetTaskResponseDto> AddTaskAsync(CreateTaskDto newTask, string userId)
    {
        var taskEntity = _mapper.Map<TaskEntity>(newTask);
        taskEntity.UserId = userId;
        await _repo.AddTaskAsync(taskEntity);
        await _repo.SaveChangesAsync();
        return _mapper.Map<GetTaskResponseDto>(taskEntity);
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var taskToDelete = await _repo.GetTaskByIdAsync(taskId);

        if (taskToDelete == null) return false;

        await _repo.DeleteTaskAsync(taskToDelete);
        await _repo.SaveChangesAsync();
        return true;


    }

    public async Task<IEnumerable<GetTaskResponseDto>> GetAllTasksAsync(string userId)
    {
        var tasks = await _repo.GetAllTasksAsync(userId);
        return _mapper.Map<IEnumerable<GetTaskResponseDto>>(tasks);
    }

    public async Task<GetTaskResponseDto> GetTaskByIdAsync(int taskId)
    {
        var task = await _repo.GetTaskByIdAsync(taskId);
        return _mapper.Map<GetTaskResponseDto>(task);

    }

    public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updatedTask)
    {
        var exsistTingTask = await _repo.GetTaskByIdAsync(taskId);

        if (exsistTingTask == null) return false;

   
        _mapper.Map(updatedTask, exsistTingTask);

        await _repo.UpdateTaskAsync(exsistTingTask);
        await _repo.SaveChangesAsync(); 

        return true;

    }
}

