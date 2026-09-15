using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Mappings;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable(
            "TaskItems",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_TaskItems_ProgressPercentage",
                    "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");

                tableBuilder.HasCheckConstraint(
                    "CK_TaskItems_DueDateUtc",
                    "[DueDateUtc] IS NULL OR [StartDateUtc] IS NULL OR [DueDateUtc] >= [StartDateUtc]");
            });

        builder.HasKey(taskItem => taskItem.Id);

        builder.Property(taskItem => taskItem.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(taskItem => taskItem.Description)
            .HasMaxLength(4000);

        builder.Property(taskItem => taskItem.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(taskItem => taskItem.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(taskItem => taskItem.ProgressPercentage)
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasOne(taskItem => taskItem.Project)
            .WithMany(project => project.Tasks)
            .HasForeignKey(taskItem => taskItem.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(taskItem => taskItem.AssignedUser)
            .WithMany(user => user.AssignedTasks)
            .HasForeignKey(taskItem => taskItem.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(taskItem => taskItem.CreatedByUser)
            .WithMany(user => user.CreatedTasks)
            .HasForeignKey(taskItem => taskItem.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(taskItem => new { taskItem.ProjectId, taskItem.Status });
        builder.HasIndex(taskItem => new { taskItem.AssignedUserId, taskItem.Status });
        builder.HasIndex(taskItem => taskItem.DueDateUtc);
    }
}