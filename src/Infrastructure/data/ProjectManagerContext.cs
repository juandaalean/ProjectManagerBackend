using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.data
{
    public class ProjectManagerContext : DbContext
    {
        public ProjectManagerContext(DbContextOptions<ProjectManagerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectManagerContext).Assembly);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
    }
}