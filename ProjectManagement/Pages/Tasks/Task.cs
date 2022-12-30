using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Projects;
using RazorClassLibrary;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectManagement.Pages.Tasks
{
    public class TaskView : BaseView<Task>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        [Field(DisplayName = "Проект"), Bind("Project.Name")]
        public string ProjectFullName { get; set; }

        [Field(Serviced = true)]
        public int Status { get; set; }

        [Field(DisplayName = "Статус"), Sort("Status")]
        public string StatusName => $"{Status} - {((TaskStatus)Status).GetDisplayName()}";

        [Field(DisplayName = "Дата начала", Format = "g")]
        public DateTime? StartDate { get; set; }

        [Field(DisplayName = "Дата окончания", Format = "g")]
        public DateTime? EndDate { get; set; }

        public override string GetName() => "Задача";

        public override string GetNames() => "Задачи";

        public override string GetEntityName() => "task";

        public override string GetEntityNames() => "tasks";

        public override IQueryable<Task> Include(IQueryable<Task> dbSet)
        {
            return dbSet.Include(x => x.Project);
        }
    }

    public class TaskAllView : TaskView
    {
        private MyContext _context;

        public TaskAllView()
        {

        }

        public TaskAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var queryable = Include(_context.Tasks);

            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return queryable;
        }
    }

    public class TasksByProjectView : TaskAllView
    {
        [Field(DisplayName = "Проект", Serviced = true)]
        public new string ProjectFullName { get; set; }

        private MyContext _context;

        private int _projectId;

        public TasksByProjectView()
        {

        }

        public TasksByProjectView(MyContext context, int projectId) : base(context)
        {
            _context = context;
            _projectId = projectId;
        }

        public override IQueryable GetData(string filter)
        {
            return base.GetData(filter).Cast<Task>().Where(x => x.ProjectId == _projectId);
        }
    }

    public class Task : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ProjectId { get; set; }

        public Project Project { get; set; }

        public int Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        //public string Url => $"clients/{Id}";

        public override string ToString() => $"{Name}";
    }

    public enum TaskStatus
    {
        [Display(Name = "Новая")]
        New = 0,

        [Display(Name = "В работе")]
        InWork = 1,

        [Display(Name = "Готово")]
        Done = 2,
    }

    public class TaskFactory : BaseFactory<Task>
    {
        public TaskFactory(DbContext context) : base(context)
        {
        }
    }
}
