using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public ICollection<TaskItem> Tasks {get; set;} = new List<TaskItem>();

        public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    }
}