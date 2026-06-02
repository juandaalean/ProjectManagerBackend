using Domain.Enum;

namespace Domain.Entities
{
    public class Sprint
    {
        public Guid SprintId { get; set; }
        public string Name { get; set; } = null!;
        public string? Goal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SprintState State { get; set; } = SprintState.Planned;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
