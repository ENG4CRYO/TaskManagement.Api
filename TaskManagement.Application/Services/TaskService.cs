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

    public async Task<bool> DeleteTaskAsync(int taskId, string userId)
    {
        var taskToDelete = await _repo.GetTaskByIdAsync(taskId,userId);

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

    public async Task<GetTaskResponseDto> GetTaskByIdAsync(int taskId,string userId)
    {

        var task = await _repo.GetTaskByIdAsync(taskId,userId);
        return _mapper.Map<GetTaskResponseDto>(task);

    }

    public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updatedTask, string userId)
    {
        var exsistTingTask = await _repo.GetTaskByIdAsync(taskId,userId);

        if (exsistTingTask == null) return false;

   
        _mapper.Map(updatedTask, exsistTingTask);

        await _repo.UpdateTaskAsync(exsistTingTask);
        await _repo.SaveChangesAsync(); 

        return true;

    }
}

