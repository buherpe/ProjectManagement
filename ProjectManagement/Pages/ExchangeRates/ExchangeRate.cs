using ProjectManagement.Pages.Settings;
using RazorClassLibrary;

namespace ProjectManagement.Pages.ExchangeRates
{
    public class ExchangeRateView : BaseView<ExchangeRate>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Код")]
        public string Code { get; set; }

        [Field(DisplayName = "Курс")]
        public decimal Rate { get; set; }

        public override string GetName() => "Курс валюты";

        public override string GetNames() => "Курсы валют";

        public override string GetEntityName() => "exchangeRate";

        public override string GetEntityNames() => "exchangeRates";
    }

    public class ExchangeRateAllView : ExchangeRateView
    {
        private MyContext _context;

        public ExchangeRateAllView()
        {

        }

        public ExchangeRateAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var exchangeRates = _context.ExchangeRates.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                exchangeRates = exchangeRates.Where(x => x.Code.ToLower().Contains(filter.ToLower()));
            }

            return exchangeRates;
        }
    }

    public class ExchangeRate : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public decimal Rate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedBy { get; set; }

        public override string ToString() => $"{Code}";
    }
}
