using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Entities
{
    public class UserProject
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    
        public UserRol RoleInProject { get; set; }
    }
}