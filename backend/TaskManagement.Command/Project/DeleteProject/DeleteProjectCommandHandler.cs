using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.Domain.Contracts;

namespace TaskManagement.Command.Project.DeleteProject;

public sealed class DeleteProjectCommandHandler(
    IProjectRepository projectRepository,
    ITaskItemRepository taskItemRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProjectCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetByIdAsync(command.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        var taskItems = await taskItemRepository.ListByProjectIdAsync(project.Id, cancellationToken);
        foreach (var taskItem in taskItems)
        {
            await activityLogRepository.DeleteByTaskItemIdAsync(taskItem.Id, cancellationToken);
        }

        await activityLogRepository.DeleteByProjectIdAsync(project.Id, cancellationToken);

        projectRepository.Remove(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}