using RazorClassLibrary;

namespace ProjectManagement.Pages.ProjectStatuses
{
    public class ProjectStatusView : BaseView<ProjectStatus, ProjectStatusFactory, MyContext>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string? Name { get; set; }

        public override string GetName() => "Статус проекта";

        public override string GetNames() => "Статусы проекта";

        public override string GetEntityName() => "projectStatus";

        public override string GetEntityNames() => "projectStatuses";
    }

    public class ProjectStatusAllView : ProjectStatusView
    {
        public override IQueryable GetData(string? filter)
        {
            var projectStatuses = new MyContext().ProjectStatuses.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                projectStatuses = projectStatuses.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return projectStatuses.OrderByDescending(x => x.Id);
        }
    }

    public class ProjectStatus : IEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        //public string Url => $"clients/{Id}";

        public override string ToString() => $"{Name}";
    }

    public class ProjectStatusFactory : BaseFactory<ProjectStatus, MyContext>
    {

    }
}
