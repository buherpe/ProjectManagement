using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Projects;
using RazorClassLibrary;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectManagement.Pages.Headaches
{
    public class HeadacheView : BaseView<Headache>
    {
        [Field(DisplayName = "#")]
        public int Id { get; set; }

        [Field(DisplayName = "Дата", Format = "d")]
        public DateTime? DateTime { get; set; }

        [Field(Serviced = true)]
        public bool? IsHeadache { get; set; }

        [Field(DisplayName = "Головная боль"), Sort("IsHeadache")]
        public string IsHeadacheString => IsHeadache.HasValue && IsHeadache.Value ? "Да" : "";

        [Field(Serviced = true)]
        public bool? Aura { get; set; }

        [Field(DisplayName = "Аура"), Sort("Aura")]
        public string AuraString => Aura.HasValue && Aura.Value ? "Да" : "";

        [Field(DisplayName = "Препарат")]
        public string Drug { get; set; }

        [Field(Serviced = true)]
        public bool? DrugEffectiveness { get; set; }

        [Field(DisplayName = "Эффективность препарата"), Sort("DrugEffectiveness")]
        public string DrugEffectivenessString => DrugEffectiveness.HasValue && DrugEffectiveness.Value ? "Да" : "";

        [Field(DisplayName = "Интенсивность боли")]
        public int? PainPower { get; set; }

        [Field(Serviced = true)]
        public int Localization { get; set; }

        [Field(DisplayName = "Локализация"), Sort("Localization")]
        public string LocalizationName => 
            $"{Localization} - {((HeadacheLocalization)Localization).GetDisplayName()}";

        [Field(Serviced = true)]
        public int Nature { get; set; }

        [Field(DisplayName = "Характер"), Sort("Nature")]
        public string NatureName => $"{Nature} - {((HeadacheNature)Nature).GetDisplayName()}";

        [Field(Serviced = true)]
        public bool? IncreasedOnExertion { get; set; }

        [Field(DisplayName = "Усиление при нагрузке"), Sort("IncreasedOnExertion")]
        public string IncreasedOnExertionString =>
            IncreasedOnExertion.HasValue && IncreasedOnExertion.Value ? "Да" : "";

        [Field(Serviced = true)]
        public bool? Nausea { get; set; }

        [Field(DisplayName = "Тошнота"), Sort("Nausea")]
        public string NauseaString => Nausea.HasValue && Nausea.Value ? "Да" : "";

        [Field(Serviced = true)]
        public bool? Photophobia { get; set; }

        [Field(DisplayName = "Фотофобия"), Sort("Photophobia")]
        public string PhotophobiaString => Photophobia.HasValue && Photophobia.Value ? "Да" : "";

        [Field(Serviced = true)]
        public bool? Phonophobia { get; set; }

        [Field(DisplayName = "Фонофобия"), Sort("Phonophobia")]
        public string PhonophobiaString => Phonophobia.HasValue && Phonophobia.Value ? "Да" : "";

        [Field(DisplayName = "Триггеры")]
        public string Triggers { get; set; }

        [Field(DisplayName = "Начало")]
        public string Start { get; set; }

        [Field(DisplayName = "Конец")]
        public string End { get; set; }

        [Field(DisplayName = "Комментарий")]
        public string Comment { get; set; }

        public override string GetName() => "Головная боль";

        public override string GetNames() => "Головные боли";

        public override string GetEntityName() => "headache";

        public override string GetEntityNames() => "headaches";
    }

    public class HeadacheAllView : HeadacheView
    {
        private MyContext _context;

        public HeadacheAllView()
        {

        }

        public HeadacheAllView(MyContext context)
        {
            _context = context;
        }

        public override IQueryable GetData(string filter)
        {
            var queryable = Include(_context.Headaches);

            if (!string.IsNullOrEmpty(filter))
            {
                //queryable = queryable.Where(x => x.Name.ToLower().Contains(filter.ToLower()));
            }

            return queryable;
        }
    }

    public class Headache : IEntity, ICreatedModified
    {
        public int Id { get; set; }

        public DateTime? DateTime { get; set; }

        public bool? IsHeadache { get; set; }

        public bool? Aura { get; set; }

        public string Drug { get; set; }

        public bool? DrugEffectiveness { get; set; }

        public int? PainPower { get; set; }

        public int Localization { get; set; }

        public int Nature { get; set; }

        public bool? IncreasedOnExertion { get; set; }

        public bool? Nausea { get; set; }

        public bool? Photophobia { get; set; }

        public bool? Phonophobia { get; set; }

        public string Triggers { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string Comment { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public int? ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public override string ToString() => $"{Id}";
    }

    public enum HeadacheLocalization
    {
        [Display(Name = "Нет")]
        None = 0,

        [Display(Name = "Вся")]
        All = 1,

        [Display(Name = "Слева")]
        Left = 2,

        [Display(Name = "Справа")]
        Right = 3,
    }

    public enum HeadacheNature
    {
        [Display(Name = "Нет")]
        None = 0,

        [Display(Name = "Давила")]
        Pressure = 1,

        [Display(Name = "Ныла")]
        Aching = 2,

        [Display(Name = "Пульсирующий")]
        Throbbing = 3,

        [Display(Name = "Резала")]
        Cutting = 4,
    }
}
