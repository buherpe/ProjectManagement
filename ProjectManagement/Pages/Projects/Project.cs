using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.ProjectStatuses;
using RazorClassLibrary;

namespace ProjectManagement.Pages.Projects
{
    public class ProjectView : BaseView<Project, ProjectFactory, MyContext>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string? Name { get; set; }

        [Field(DisplayName = "Ссылка")]
        public string? Link { get; set; }

        [Field(DisplayName = "Статус"), Bind("Status.Name")]
        public string? StatusName { get; set; }

        public override string GetName() => "Проект";

        public override string GetNames() => "Проекты";

        public override string GetEntityName() => "project";

        public override string GetEntityNames() => "projects";
    }

    public class ProjectAllView : ProjectView
    {
        public override IQueryable GetData(string? filter)
        {
            var projects = new MyContext().Projects.Include(x => x.Status).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                projects = projects.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return projects.OrderByDescending(x => x.Id);
        }
    }

    public class Project : IEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Link { get; set; }

        public int? StatusId { get; set; }

        public ProjectStatus? Status { get; set; }

        //public string Url => $"clients/{Id}";

        public override string ToString() => $"{Name}";
    }

    public class ProjectFactory : BaseFactory<Project, MyContext>
    {

    }
}
