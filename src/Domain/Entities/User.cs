using Domain.Enum;

namespace Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRol Rol { get; set; }

        public ICollection<UserProject> UserProjects {get; set;} = new List<UserProject>();
        public ICollection<TaskItem> AssignedTasks {get; set;} = new List<TaskItem>();
        public ICollection<Comment> Comments {get; set;} = new List<Comment>();
    }
}