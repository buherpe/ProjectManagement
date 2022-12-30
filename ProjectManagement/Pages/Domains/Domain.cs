using RazorClassLibrary;

namespace ProjectManagement.Pages.Domains
{
    public class DomainView : BaseView<Domain>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Наименование")]
        public string Name { get; set; }

        [Field(DisplayName = "Истекает", Format = "d")]
        public DateTime? ExpiryDate { get; set; }

        [Field(DisplayName = "Осталось дней")]
        public double? ExpiryDate2 => ExpiryDate?.Subtract(DateTime.Today).TotalDays;

        public override string GetName() => "Домен";

        public override string GetNames() => "Домены";

        public override string GetEntityName() => "domain";

        public override string GetEntityNames() => "domains";
    }

    public class DomainAllView : DomainView
    {
        private MyContext _context;

        public DomainAllView()
        {

        }

        public DomainAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var domains = _context.Domains.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                domains = domains.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return domains;
        }
    }

    public class Domain : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public long FreenomDomainId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public override string ToString() => $"{Name}";
    }
}
