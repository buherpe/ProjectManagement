using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Projects;
using ProjectManagement.Pages.ProjectStatuses;
using ProjectManagement.Pages.Domains;
using RazorClassLibrary;
using ProjectManagement.Pages.Settings;
using ProjectManagement.Pages.Currencies;

namespace ProjectManagement
{
    public class MyContext : DbContext2, IUsers
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
            
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", false, true)
        //        .Build();

        //    optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        //}

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectManagement.Pages.Tasks.Task> Tasks { get; set; }

        public DbSet<ProjectStatus> ProjectStatuses { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Domain> Domains { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Currency> Currencies { get; set; }
    }
}
