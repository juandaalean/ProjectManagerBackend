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
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public ICollection<TaskItem> Tasks {get; set;} = [];

        public ICollection<UserProject> UserProjects { get; set; } = [];
    }
}