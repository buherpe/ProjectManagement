﻿using Microsoft.EntityFrameworkCore;
using ProjectManagement.Pages.Currencies;
using RestSharp;

namespace ProjectManagement
{
    public class ExchangeRateUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ILogger<ExchangeRateUpdater> _logger;

        private readonly IConfiguration _configuration;

        public ExchangeRateUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<ExchangeRateUpdater> logger, IConfiguration configuration)
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
                    var exchangeRateLastUpdateSetting = await context.Settings.FirstOrDefaultAsync(x =>
                        x.Name == "ExchangeRateLastUpdate");

                    //_logger.LogInformation($"ExecuteAsync: Последнее обновление курса: " +
                    //    $"{exchangeRateLastUpdateSetting.Value}");

                    if (exchangeRateLastUpdateSetting == null ||
                        string.IsNullOrEmpty(exchangeRateLastUpdateSetting.Value) ||
                        DateTime.Now - DateTime.Parse(exchangeRateLastUpdateSetting.Value)
                            > TimeSpan.FromMinutes(_configuration.GetValue<int>("ExchangeRateApi:UpdateRate")))
                    {
                        _logger.LogInformation($"ExecuteAsync: Курс протух, обновляем");

                        var client = new RestClient(_configuration.GetSection("ExchangeRateApi:Url").Value);

                        var request = new RestRequest();

                        var confHeader = _configuration.GetValue<string>("ExchangeRateApi:Header");
                        request.AddHeader(confHeader.Split(':')[0], confHeader.Split(':')[1]);

                        _logger.LogInformation($"ExecuteAsync: Отправляем запрос к апи");
                        var exchangeRateApiResponseResponse = await client.GetAsync(request);
                        var exchangeRateApiResponse = client.Deserialize<ExchangeRateApiResponse>(exchangeRateApiResponseResponse);
                        _logger.LogInformation($"ExecuteAsync: Ответ апи: {exchangeRateApiResponseResponse.Content}");
                        
                        foreach (var apiRate in exchangeRateApiResponse.Data.Rates)
                        {
                            var dbRate = await context.Currencies.FirstOrDefaultAsync(x => x.Code == apiRate.Key)
                                ?? new Currency();

                            dbRate.Code = apiRate.Key;
                            dbRate.Rate = apiRate.Value;

                            context.Update(dbRate);
                        }

                        exchangeRateLastUpdateSetting.Value = $"{DateTime.Now:O}";

                        _logger.LogInformation($"ExecuteAsync: Сохраняем");
                        await context.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"ExecuteAsync: Сохранено. Обновлено курсов: {exchangeRateApiResponse.Data.Rates.Count}");
                    }

                    await Task.Delay(60_000, stoppingToken);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExecuteAsync error");
                }
            }
        }
    }

    public class ExchangeRateApiResponse
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
