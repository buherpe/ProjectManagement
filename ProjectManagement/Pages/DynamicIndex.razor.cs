using ProjectManagement.Pages.Projects;
using ProjectManagement.Pages.ProjectStatuses;
using ProjectManagement.Pages.Tasks;
using ProjectManagement.Pages.Users;
using RazorClassLibrary;

namespace ProjectManagement.Pages
{
    public partial class DynamicIndex
    {
        public static List<IView> Views { get; set; } = new List<IView>();

        public DynamicIndex()
        {
            Views.Add(new ProjectAllView());
            Views.Add(new ProjectStatusAllView());
            Views.Add(new TaskAllView());
            Views.Add(new UserAllView());
        }
    }
}
