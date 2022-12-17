using ProjectManagement.Pages.Domains;
using RazorClassLibrary;

namespace ProjectManagement.Pages.Settings
{
    public class SettingView : BaseView<Setting>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        [Field(DisplayName = "Значение")]
        public string Value { get; set; }

        public override string GetName() => "Настройка";

        public override string GetNames() => "Настройки";

        public override string GetEntityName() => "setting";

        public override string GetEntityNames() => "settings";
    }

    public class SettingAllView : SettingView
    {
        private MyContext _context;

        public SettingAllView()
        {

        }

        public SettingAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var settings = _context.Settings.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                settings = settings.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return settings;
        }
    }

    public class Setting : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedBy { get; set; }

        public override string ToString() => $"{Name}";
    }
}
