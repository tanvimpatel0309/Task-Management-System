using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Mappings;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Content)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasOne(comment => comment.TaskItem)
            .WithMany(taskItem => taskItem.Comments)
            .HasForeignKey(comment => comment.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(comment => comment.User)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(comment => new { comment.TaskItemId, comment.CreatedAtUtc });
    }
}