using Domain.Enum;

namespace Domain.Entities
{
    public class TaskItem
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskState State { get; set; }
        public TaskPriority Priority { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid AssignedUserId { get; set; }
        public User AssignedUser { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}