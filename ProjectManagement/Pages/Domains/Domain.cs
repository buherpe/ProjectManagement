using RazorClassLibrary;

namespace ProjectManagement.Pages.Domains
{
    public class DomainView : BaseView<Domain, DomainFactory, MyContext>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        [Field(DisplayName = "Истекает", Format = "d")]
        public DateTime ExpiryDate { get; set; }

        [Field(DisplayName = "Осталось дней")]
        public double ExpiryDate2 => ExpiryDate.Subtract(DateTime.Today).TotalDays;

        public override string GetName() => "Домен";

        public override string GetNames() => "Домены";

        public override string GetEntityName() => "domain";

        public override string GetEntityNames() => "domains";
    }

    public class DomainAllView : DomainView
    {
        public override IQueryable GetData(string filter)
        {
            var domains = new MyContext().Domains.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                domains = domains.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return domains.OrderByDescending(x => x.Id);
        }
    }

    public class Domain : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime ExpiryDate { get; set; }

        public long FreenomDomainId { get; set; }

        public override string ToString() => $"{Name}";
    }

    public class DomainFactory : BaseFactory<Domain, MyContext>
    {

    }
}
