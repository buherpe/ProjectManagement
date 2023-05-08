//using Microsoft.EntityFrameworkCore;
//using ProjectManagement.Pages.Currencies;
//using RazorClassLibrary;
//using System.Globalization;
//using System.Text.RegularExpressions;
//using Telegram.Bot;
//using Telegram.Bot.Polling;
//using Telegram.Bot.Types.Enums;

//namespace ProjectManagement
//{
//    public class TelegramBotWorker : BackgroundService
//    {
//        private readonly ILogger<TelegramBotWorker> _log;

//        private readonly IServiceScopeFactory _serviceScopeFactory;

//        private readonly IConfiguration _configuration;

//        public TelegramBotWorker(IServiceScopeFactory serviceScopeFactory,
//            IConfiguration configuration,
//            ILogger<TelegramBotWorker> log)
//        {
//            _serviceScopeFactory = serviceScopeFactory;
//            _configuration = configuration;
//            _log = log;
//        }

//        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
//        {
//            try
//            {
//                using var scope = _serviceScopeFactory.CreateScope();
//                var context = scope.ServiceProvider.GetRequiredService<MyContext>();
//                var telegramBotWorkerEnabled = bool.Parse((await context.Settings.FirstOrDefaultAsync(x =>
//                    x.Name == "CurrencyConverterTelegramBotWokerEnabled")).Value);

//                _log.LogInformation($"telegramBotWorkerEnabled: {telegramBotWorkerEnabled}");

//                if (!telegramBotWorkerEnabled)
//                {
//                    return;
//                }

//                _log.LogInformation($"ExecuteAsync: Запускаем тг бота");

//                var telegramBotToken = await context.Settings.FirstOrDefaultAsync(x =>
//                    x.Name == "CurrencyConverterTelegramBotToken");

//                string token;

//                if (telegramBotToken.Encrypted)
//                {
//                    var aesGcmService = new AesGcmService(_configuration["EncryptionPassword"]);
//                    token = aesGcmService.Decrypt(telegramBotToken.Value);
//                }
//                else
//                {
//                    token = telegramBotToken.Value;
//                }

//                var botClient = new TelegramBotClient(token);

//                var me = await botClient.GetMeAsync(cancellationToken);

//                _log.LogInformation($"ExecuteAsync: Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

//                using var cts = new CancellationTokenSource();

//                botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cancellationToken: cts.Token);
//            }
//            catch (Exception e)
//            {
//                _log.LogError(e, "ExecuteAsync");
//            }
//        }

//        private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
//        {
//            _log.LogError(exception, $"HandleErrorAsync");

//            await Task.Delay(1000, cancellationToken);
//        }

//        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
//        {
//            try
//            {
//                if (update.Type != UpdateType.Message)
//                    return;
//                if (update.Message.Type != MessageType.Text)
//                    return;

//                var chatId = update.Message.Chat.Id;

//                var defaultCurrencies = $"usd, rub";

//                using var scope = _serviceScopeFactory.CreateScope();
//                var context = scope.ServiceProvider.GetRequiredService<MyContext>();
//                context.CurrentUserId = 16;

//                var allCurrencies = await context.Currencies.ToListAsync();

//                var currenciesWithSymbols = allCurrencies.Where(x => !string.IsNullOrEmpty(x.Symbols)).ToList();
//                var sumAndCurrencies = ParseSumAndCurrency(currenciesWithSymbols, update.Message.Text);

//                var groupSetting = await context.CurrencyConverterChatSettings
//                    .FirstOrDefaultAsync(x => x.ChatId == update.Message.Chat.Id);

//                var sourceCurrencyStr = (groupSetting?.Currencies ?? defaultCurrencies)
//                    .Split(',').Select(x => x.Trim()).Select(x => x.ToUpperInvariant()).ToList();

//                sumAndCurrencies = sumAndCurrencies.Where(x => sourceCurrencyStr.Any(c => x.Currency.Code == c))
//                    .ToList();

//                if (!sumAndCurrencies.Any())
//                {
//                    return;
//                }

//                _log.LogInformation($"HandleUpdateAsync: Чат: {chatId}, юзер: {update.Message.From.Id}, текст: {update.Message.Text}");

//                var str = $"";

//                foreach (var sumAndCurrency in sumAndCurrencies)
//                {
//                    _log.LogDebug($"HandleUpdateAsync: sumAndCurrency: {sumAndCurrency}");

//                    var targetCurrenciesStr = (groupSetting?.Currencies ?? defaultCurrencies)
//                        .Split(',').Select(x => x.Trim()).Select(x => x.ToUpperInvariant())
//                        .Where(x => x != sumAndCurrency.Currency.Code)
//                        .ToList();

//                    var targetCurrencies = targetCurrenciesStr
//                        .Select(x => allCurrencies.FirstOrDefault(c => c.Code == x))
//                        .ToList();

//                    var sums = new List<string>();

//                    foreach (var targetCurrency in targetCurrencies)
//                    {
//                        _log.LogDebug($"HandleUpdateAsync: targetCurrency: {targetCurrency}");

//                        var source = sumAndCurrency.Currency.Rate;
//                        var target = targetCurrency.Rate;
//                        var exchangeRate = target / source;

//                        var sum = sumAndCurrency.Sum * exchangeRate;
//                        var symbol = targetCurrency.Symbols?.Split('|').FirstOrDefault() ?? targetCurrency.Code;

//                        if (sum > 10)
//                        {
//                            sums.Add($"{Math.Round(sum, 0).ToString("N0", CultureInfo.InvariantCulture)}{symbol}");
//                        }
//                        else
//                        {
//                            sums.Add($"{Math.Round(sum, 2).ToString("N2", CultureInfo.InvariantCulture)}{symbol}");
//                        }
//                    }

//                    str += $"{string.Join(" | ", sums)}\n";
//                }

//                _log.LogDebug($"HandleUpdateAsync: str: {str}");
//                await botClient.SendTextMessageAsync(chatId, $"{str}", cancellationToken: cancellationToken, replyToMessageId: update.Message.MessageId, disableNotification: true);

//                if (groupSetting == null)
//                {
//                    groupSetting = new Pages.CurrencyConverterChatSettings.CurrencyConverterChatSetting();
//                    context.Add(groupSetting);
//                }

//                groupSetting.ChatId = chatId;
//                groupSetting.ConvertCount += 1;

//                await context.SaveAsync(true, cancellationToken);
//            }
//            catch (Exception e)
//            {
//                _log.LogError(e, $"HandleUpdateAsync");
//            }
//        }

//        public List<ParsingResult> ParseSumAndCurrency(List<Currency> currencies, string input)
//        {
//            var result = new List<ParsingResult>();

//            var possibleCurrencySymbols = currencies.Select(x => x.Symbols).ToList();
//            var possibleCurrencySymbolsStr = string.Join("|", possibleCurrencySymbols);
//            _log.LogDebug($"ParseSumAndCurrency: possibleCurrencySymbolsStr: {possibleCurrencySymbolsStr}");

//            var pattern = @$"\b(?'sumWcur'(?'sum'(?!0+\.00)(?=.{{1,9}}(\.|$| ))(?!0(?!\.))\d{{1,3}}((,| |)\d{{3}})*(\.\d+)?) ?(?'cur'{possibleCurrencySymbolsStr}))(\s|$)";
//            _log.LogDebug($"ParseSumAndCurrency: pattern: {pattern}");
//            var matches = Regex.Matches(input, pattern, RegexOptions.Multiline);

//            foreach (Match match in matches)
//            {
//                var currency = currencies
//                    .FirstOrDefault(x => Regex.IsMatch(match.Groups["cur"].Value, $"^({x.Symbols})$"));

//                var parsingResult = new ParsingResult();

//                parsingResult.Currency = currency;
//                parsingResult.Sum = decimal.Parse(match.Groups["sum"].Value);

//                result.Add(parsingResult);
//            }

//            return result;
//        }

//        public class ParsingResult
//        {
//            public Currency Currency { get; set; }

//            public decimal Sum { get; set; }

//            public override string ToString() => $"{Sum}{Currency?.Code}";
//        }
//    }
//}
