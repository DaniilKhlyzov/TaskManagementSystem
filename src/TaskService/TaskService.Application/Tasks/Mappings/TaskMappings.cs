using AutoMapper;
using TaskService.Application.Tasks.Dtos;
using TaskService.Domain;

namespace TaskService.Application.Tasks.Mappings;

public class TaskMappings : Profile
{
    public TaskMappings()
    {
        CreateMap<Job, TaskListItemDto>();
        CreateMap<Job, TaskDetailsDto>();
        CreateMap<CreateTaskDto, Job>();
        CreateMap<UpdateTaskDto, Job>();
    }
}


