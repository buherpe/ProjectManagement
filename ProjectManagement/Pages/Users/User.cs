using RazorClassLibrary;

namespace ProjectManagement.Pages.Users
{
    public class UserView : BaseView<User>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Логин")]
        public string Username { get; set; }

        [Field(DisplayName = "ФИО")]
        public string Fullname { get; set; }

        public override string GetName() => "Пользователь";

        public override string GetNames() => "Пользователи";

        public override string GetEntityName() => "user";

        public override string GetEntityNames() => "users";
    }

    public class UserAllView : UserView
    {
        private MyContext _context;

        public UserAllView()
        {

        }

        public UserAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                users = users.Where(x => x.Username.ToLower().Contains(filter.ToLower()));
            }

            return users.OrderByDescending(x => x.Id);
        }
    }
}
