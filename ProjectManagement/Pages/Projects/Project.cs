using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.ProjectStatuses;
using RazorClassLibrary;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Pages.Projects
{
    public class ProjectView : BaseView<Project>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        [Field(DisplayName = "Ссылка")]
        public string Link { get; set; }

        [Field(DisplayName = "Статус"), Bind("Status.Name")]
        public string StatusName { get; set; }

        //[Field(DisplayName = "Описание")]
        //public string Description { get; set; }

        public override string GetName() => "Проект";

        public override string GetNames() => "Проекты";

        public override string GetEntityName() => "project";

        public override string GetEntityNames() => "projects";

        public override IQueryable<Project> Include(IQueryable<Project> queryable)
        {
            return queryable.Include(x => x.Status);
        }
    }

    public class ProjectAllView : ProjectView
    {
        private readonly MyContext _context;

        public ProjectAllView()
        {

        }

        public ProjectAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var queryable = Include(_context.Projects);

            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(filter.ToLower()) ||
                                                 x.Link.ToLower().Contains(filter.ToLower()) ||
                                                 x.Description.ToLower().Contains(filter.ToLower()));
            }

            return queryable;
        }
    }

    public class Project : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Link { get; set; }

        public int? StatusId { get; set; }

        public ProjectStatus Status { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public override string ToString() => $"{Name}";
    }
}
