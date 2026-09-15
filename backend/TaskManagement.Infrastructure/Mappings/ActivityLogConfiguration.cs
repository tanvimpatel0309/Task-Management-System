using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Mappings;

public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");

        builder.HasKey(activityLog => activityLog.Id);

        builder.Property(activityLog => activityLog.Action)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(activityLog => activityLog.Details)
            .HasMaxLength(4000);

        builder.Property(activityLog => activityLog.OldValue)
            .HasMaxLength(2000);

        builder.Property(activityLog => activityLog.NewValue)
            .HasMaxLength(2000);

        builder.Property(activityLog => activityLog.OccurredAtUtc)
            .IsRequired();

        builder.HasOne(activityLog => activityLog.User)
            .WithMany(user => user.ActivityLogs)
            .HasForeignKey(activityLog => activityLog.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(activityLog => activityLog.Project)
            .WithMany(project => project.ActivityLogs)
            .HasForeignKey(activityLog => activityLog.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(activityLog => activityLog.TaskItem)
            .WithMany(taskItem => taskItem.ActivityLogs)
            .HasForeignKey(activityLog => activityLog.TaskItemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(activityLog => new { activityLog.ProjectId, activityLog.OccurredAtUtc });
        builder.HasIndex(activityLog => new { activityLog.TaskItemId, activityLog.OccurredAtUtc });
    }
}