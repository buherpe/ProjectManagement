using ProjectManagement.Pages.Settings;
using RazorClassLibrary;

namespace ProjectManagement.Pages.CurrencyConverterChatSettings
{
    public class CurrencyConverterChatSettingView : BaseView<CurrencyConverterChatSetting>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Чат")]
        public long ChatId { get; set; }

        [Field(DisplayName = "Валюты")]
        public string Currencies { get; set; }

        public override string GetName() => "Конвертер валют → Настройки чата";

        public override string GetNames() => "Конвертер валют → Настройки чатов";

        public override string GetEntityName() => "currencyConverterChatSetting";

        public override string GetEntityNames() => "currencyConverterChatSettings";
    }

    public class CurrencyConverterChatSettingAllView : CurrencyConverterChatSettingView
    {
        private MyContext _context;

        public CurrencyConverterChatSettingAllView()
        {

        }

        public CurrencyConverterChatSettingAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var currencyConverterChatSettings = _context.CurrencyConverterChatSettings.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                currencyConverterChatSettings = currencyConverterChatSettings
                    .Where(x => x.Currencies.ToLower().Contains(filter.ToLower()));
            }

            return currencyConverterChatSettings;
        }
    }

    public class CurrencyConverterChatSetting : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public long ChatId { get; set; }

        public string Currencies { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public override string ToString() => $"{Id}";
    }
}
