using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Projects;
using RazorClassLibrary;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectManagement.Pages.Tasks
{
    public class TaskView : BaseView<Task, TaskFactory, MyContext>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        [Field(DisplayName = "Проект"), Bind("Project.Name")]
        public string ProjectFullName { get; set; }

        [Field(Serviced = true)]
        public int Status { get; set; }

        [Field(DisplayName = "Статус")]
        public string StatusName => $"{Status} - {((TaskStatus)Status).GetDisplayName()}";

        [Field(DisplayName = "Дата начала")]
        public DateTime? StartDate { get; set; }

        [Field(DisplayName = "Дата конца")]
        public DateTime? EndDate { get; set; }

        public override string GetName() => "Задача";

        public override string GetNames() => "Задачи";

        public override string GetEntityName() => "task";

        public override string GetEntityNames() => "tasks";
    }

    public class TaskAllView : TaskView
    {
        public override IQueryable GetData(string filter)
        {
            var clients = new MyContext().Tasks.Include(x => x.Project).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                clients = clients.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return clients.OrderByDescending(x => x.Id);
        }
    }

    public class TasksByProjectView : TaskAllView
    {
        [Field(DisplayName = "Проект", Serviced = true)]
        public new string ProjectFullName { get; set; }

        private int _projectId;

        public TasksByProjectView()
        {

        }

        public TasksByProjectView(int projectId)
        {
            _projectId = projectId;
        }

        public override IQueryable GetData(string filter)
        {
            return base.GetData(filter).Cast<Task>().Where(x => x.ProjectId == _projectId);
        }
    }

    public class Task : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ProjectId { get; set; }

        public Project Project { get; set; }

        public int Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        //public string Url => $"clients/{Id}";

        public override string ToString() => $"{Name}";
    }

    public class TaskFactory : BaseFactory<Task, MyContext>
    {
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
}
