using ProjectManagement.Pages.Settings;
using RazorClassLibrary;

namespace ProjectManagement.Pages.Currencies
{
    public class CurrencyView : BaseView<Currency>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Код")]
        public string Code { get; set; }

        [Field(DisplayName = "Курс")]
        public decimal Rate { get; set; }

        [Field(DisplayName = "Символы")]
        public string Symbols { get; set; }

        public override string GetName() => "Валюта";

        public override string GetNames() => "Валюты";

        public override string GetEntityName() => "currency";

        public override string GetEntityNames() => "currencies";
    }

    public class CurrencyAllView : CurrencyView
    {
        private MyContext _context;

        public CurrencyAllView()
        {

        }

        public CurrencyAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var currency = _context.Currencies.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                currency = currency.Where(x => x.Code.ToLower().Contains(filter.ToLower()));
            }

            return currency;
        }
    }

    public class Currency : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public decimal Rate { get; set; }

        public string Symbols { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public override string ToString() => $"{Code}";
    }
}
