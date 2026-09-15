using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Task;

namespace TaskManagement.Queries.Task.GetTaskById;

public sealed class GetTaskByIdQueryHandler(ITaskItemRepository taskItemRepository)
    : IQueryHandler<GetTaskByIdQuery, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(GetTaskByIdQuery query, CancellationToken cancellationToken = default)
    {
        var taskItem = await taskItemRepository.GetByIdWithDetailsAsync(query.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        return taskItem.ToDetailsDto();
    }
}