using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        public ICollection<UserProject> UserProjects {get; set;} = [];
        public ICollection<TaskItem> AssignedTasks {get; set;} = [];
        public ICollection<Comment> Comments {get; set;} = [];
    }
}