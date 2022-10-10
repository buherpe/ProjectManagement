using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Projects;
using ProjectManagement.Pages.ProjectStatuses;

namespace ProjectManagement
{
    public class MyContext : DbContext
    {
        public MyContext()
        {

        }

        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectManagement.Pages.Tasks.Task> Tasks { get; set; }

        public DbSet<ProjectStatus> ProjectStatuses { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
