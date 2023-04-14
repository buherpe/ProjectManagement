using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Currencies;
using RazorClassLibrary;

namespace ProjectManagement
{
    //public class Q : BackgroundService
    //{
    //    private readonly IServiceScopeFactory _serviceScopeFactory;

    //    public Q(IServiceScopeFactory serviceScopeFactory)
    //    {
    //        _serviceScopeFactory = serviceScopeFactory;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            using var scope = _serviceScopeFactory.CreateScope();
    //            var context = scope.ServiceProvider.GetRequiredService<MyContext>();
    //            context.CurrentUserId = 1;
    //            var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == 123)
    //                ?? new ProjectManagement.Pages.Tasks.Task();

    //            task.Name = $"Тест админ";

    //            //context.Update(task);
    //            if (task.Id == 0)
    //            {
    //                context.Tasks.Add(task);
    //            }


    //            Console.WriteLine($"1");
    //            await context.SaveChangesAsync();
    //            Console.WriteLine($"2");

    //            await Task.Delay(10000);
    //        }
    //    }
    //}

    public class ExchangeRateUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ILogger<ExchangeRateUpdater> _logger;

        private readonly IConfiguration _configuration;

        public ExchangeRateUpdater(IServiceScopeFactory serviceScopeFactory,
            ILogger<ExchangeRateUpdater> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"ExecuteAsync: Start");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<MyContext>();
                    var exchangeRateUpdaterEnabled = bool.Parse((await context.Settings.FirstOrDefaultAsync(x =>
                                x.Name == "ExchangeRateUpdaterEnabled")).Value);

                    _logger.LogInformation($"exchangeRateUpdaterEnabled: {exchangeRateUpdaterEnabled}");

                    if (!exchangeRateUpdaterEnabled)
                    {
                        break;
                    }

                    context.CurrentUserId = 15;
                    var exchangeRateLastUpdateSetting = await context.Settings.FirstOrDefaultAsync(x =>
                        x.Name == "ExchangeRateLastUpdate");

                    //_logger.LogInformation($"ExecuteAsync: Последнее обновление курса: " +
                    //    $"{exchangeRateLastUpdateSetting.Value}");

                    var updateRateSetting = await context.Settings.FirstOrDefaultAsync(x => x.Name == "ExchangeRateApiUpdateRate");

                    if (exchangeRateLastUpdateSetting == null ||
                        string.IsNullOrEmpty(exchangeRateLastUpdateSetting.Value) ||
                        DateTime.Now - DateTime.Parse(exchangeRateLastUpdateSetting.Value)
                            > TimeSpan.FromMinutes(int.Parse(updateRateSetting.Value)))
                    {
                        _logger.LogInformation($"ExecuteAsync: Курс протух, обновляем");

                        var urlSetting = await context.Settings.FirstOrDefaultAsync(x => x.Name == "ExchangeRateApiUrl");
                        var client = new HttpClient();

                        var request = new HttpRequestMessage(HttpMethod.Get, urlSetting.Value);

                        var headerSetting = await context.Settings.FirstOrDefaultAsync(x => x.Name == "ExchangeRateApiHeader");

                        string confHeader;

                        if (headerSetting.Encrypted)
                        {
                            var aesGcmService = new AesGcmService(_configuration["EncryptionPassword"]);
                            confHeader = aesGcmService.Decrypt(headerSetting.Value);
                        }
                        else
                        {
                            confHeader = headerSetting.Value;
                        }

                        request.Headers.Add(confHeader.Split(':')[0], confHeader.Split(':')[1]);

                        _logger.LogInformation($"ExecuteAsync: Отправляем запрос к апи");
                        var exchangeRateApiResponseResponse = await client.SendAsync(request);
                        exchangeRateApiResponseResponse.EnsureSuccessStatusCode();

                        _logger.LogInformation($"ExecuteAsync: Ответ апи: {await exchangeRateApiResponseResponse.Content.ReadAsStringAsync()}");

                        var exchangeRateApiResponse = await exchangeRateApiResponseResponse.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();

                        foreach (var apiRate in exchangeRateApiResponse.Rates)
                        {
                            var dbRate = await context.Currencies.FirstOrDefaultAsync(x => x.Code == apiRate.Key)
                                ?? new Currency();

                            dbRate.Code = apiRate.Key;
                            dbRate.Rate = apiRate.Value;

                            context.AddIfNew(dbRate);
                        }

                        exchangeRateLastUpdateSetting.Value = $"{DateTime.Now:O}";

                        _logger.LogInformation($"ExecuteAsync: Сохраняем");
                        await context.SaveAsync(true, stoppingToken);
                        _logger.LogInformation($"ExecuteAsync: Сохранено. Обновлено курсов: {exchangeRateApiResponse.Rates.Count}");
                    }

                    await Task.Delay(60_000, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExecuteAsync error");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }

    public class ExchangeRateApiResponse
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
