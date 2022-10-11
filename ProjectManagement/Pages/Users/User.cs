using RazorClassLibrary;

namespace ProjectManagement.Pages.Users
{
    public class UserView : BaseView<User, UserFactory, MyContext>
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
        public override IQueryable GetData(string filter)
        {
            var users = new MyContext().Users.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                users = users.Where(x => x.Username.ToLower().Contains(filter.ToLower()));
            }

            return users.OrderByDescending(x => x.Id);
        }
    }

    public class User : IEntity
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }

        public override string ToString() => $"{Fullname}";
    }

    public class UserFactory : BaseFactory<User, MyContext>
    {

    }
}
