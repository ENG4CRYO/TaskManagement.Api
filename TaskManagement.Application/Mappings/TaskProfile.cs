using AutoMapper;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<CreateTaskDto, TaskEntity>();
        CreateMap<TaskEntity, GetTaskResponseDto>();
        CreateMap<UpdateTaskDto, TaskEntity>();   

    }
}

