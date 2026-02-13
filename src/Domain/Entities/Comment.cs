using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid TaskId { get; set; }
        public Task Task { get; set; } = null!;
    }
}