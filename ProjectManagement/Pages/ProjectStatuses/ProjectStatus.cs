using RazorClassLibrary;

namespace ProjectManagement.Pages.ProjectStatuses
{
    public class ProjectStatusView : BaseView<ProjectStatus>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        public override string GetName() => "Статус проекта";

        public override string GetNames() => "Статусы проекта";

        public override string GetEntityName() => "projectStatus";

        public override string GetEntityNames() => "projectStatuses";
    }

    public class ProjectStatusAllView : ProjectStatusView
    {
        private MyContext _context;

        public ProjectStatusAllView()
        {

        }

        public ProjectStatusAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var projectStatuses = _context.ProjectStatuses.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                projectStatuses = projectStatuses.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return projectStatuses;
        }
    }

    public class ProjectStatus : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public override string ToString() => $"{Name}";
    }
}
