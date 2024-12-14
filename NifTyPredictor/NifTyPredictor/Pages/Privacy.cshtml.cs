using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using static IndexPageModel;

namespace NifTyPredictor.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexPageModel> _logger;
        public List<TradingActivity> Data { get; set; }

        public PrivacyModel(ApplicationDbContext context, IHttpClientFactory httpClientFactory, ILogger<IndexPageModel> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var httpClient = _httpClientFactory.CreateClient("nseClient");
            for (int retry = 0; retry < 10; retry++)
            {
                try
                {
                    var response = await httpClient.GetStringAsync("https://www.nseindia.com/api/fiidiiTradeReact");
                    Data = JsonConvert.DeserializeObject<List<TradingActivity>>(response);
                   
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning($"Request error: {ex.Message}. Retrying...");

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error: {ex.Message}. Retrying...");

                }

                await Task.Delay(1000); // Delay before retry
            }
        }
    }

    public class TradingActivity
    {
        public string Category { get; set; }
        public string Date { get; set; }
        public decimal BuyValue { get; set; }
        public decimal SellValue { get; set; }
        public decimal NetValue { get; set; }
    }


}
