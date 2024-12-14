using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json; 
using NifTyPredictor;
using HtmlAgilityPack;
public class IndexPageModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexPageModel> _logger;

    public IndexPageModel(ApplicationDbContext context, IHttpClientFactory httpClientFactory, ILogger<IndexPageModel> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }


    public List<Company> Companies { get; set; } = new List<Company>();
    public List<decimal> HistoricalPrices { get; set; } = new List<decimal>(); // Add this to store historical prices
    public decimal Nifty50Index { get; private set; } 
    private const decimal BaseIndexValue = 1000m;
    private const decimal BaseMarketCap = 5498260731586.53m; // Set this to the accurate base market cap from NSE portal

    public async Task OnGetAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("nseClient");
        var initialResponse = await httpClient.GetAsync("/");
        var responseString = string.Empty;

        // Retry mechanism
        for (int retry = 0; retry < 10; retry++)
        {
            try
            {
                var response = await httpClient.GetAsync("https://www.nseindia.com/api/equity-stockIndices?index=NIFTY%2050");
                if (response.IsSuccessStatusCode)
                {
                    responseString = await response.Content.ReadAsStringAsync();
                  
                    break; // Exit loop if request is successful
                }
                else
                {
                    _logger.LogError($"Failed to fetch data. Status Code: {response.StatusCode}");
                   
                }
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

        if (string.IsNullOrEmpty(responseString))
        {
            _logger.LogError("Failed to fetch data after multiple attempts.");
          
            return;
        }
        try
        {
            var root = JsonConvert.DeserializeObject<Root>(responseString);
            Companies = root.Data.Select(d => new Company
            {
                Symbol = d.Symbol,
                Identifier = d.Identifier,
                Open = d.Open,
                DayHigh = d.DayHigh,
                DayLow = d.DayLow,
                LastPrice = d.LastPrice,
                PreviousClose = d.PreviousClose,
                Change = d.Change,
                PChange = d.PChange,
                YearHigh = d.YearHigh,
                YearLow = d.YearLow,
                ffmc = d.ffmc,
                totalTradedVolume =(double)d.totalTradedVolume / 10000000.00,
                PredictedValueEMA = PredictValueEMA(d.LastPrice),
                PredictedValueLR = PredictValueLR(d.LastPrice)
            }).ToList();
             


            // Clear existing data and add new data
            _context.Companies.RemoveRange(_context.Companies);
            _context.Companies.AddRange(Companies);
            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {

        }
    }

    private void UpdateHistoricalPrices(decimal lastPrice)
    {
        HistoricalPrices.Add(lastPrice);
        if (HistoricalPrices.Count > 100) // Keep the list size manageable
        {
            HistoricalPrices.RemoveAt(0);
        }
    }

    private decimal PredictValueEMA(decimal lastPrice)
    {
        // Add the latest price to the historical prices list
        UpdateHistoricalPrices(lastPrice);
        int period = 5;
        if (HistoricalPrices.Count < period)
        {
            return lastPrice;
        }
        return CalculateEMA(HistoricalPrices, period);
    }

    private decimal CalculateEMA(List<decimal> prices, int period)
    {
        decimal multiplier = 2m / (period + 1);
        decimal ema = prices.Take(period).Average(); // Start with the simple moving average
        foreach (var price in prices.Skip(period))
        {
            ema = ((price - ema) * multiplier) + ema;
        }
        return ema;
    }

    private decimal PredictValueLR(decimal lastPrice)
    {
        UpdateHistoricalPrices(lastPrice);
        if (HistoricalPrices.Count < 2)
        {
            return lastPrice;
        }
        return CalculateLinearRegression(HistoricalPrices);
    }

    private decimal CalculateLinearRegression(List<decimal> prices)
    {
        int count = prices.Count;
        decimal sumX = count * (count - 1) / 2; // Sum of indices
        decimal sumY = prices.Sum(); // Sum of prices
        decimal sumXY = 0; // Sum of price * index
        decimal sumX2 = count * (count + 1) * (2 * count + 1) / 6; // Sum of indices squared

        for (int i = 0; i < count; i++)
        {
            sumXY += i * prices[i];
        }

        decimal slope = (count * sumXY - sumX * sumY) / (count * sumX2 - sumX * sumX);
        decimal intercept = (sumY - slope * sumX) / count;

        // Predict the next value
        decimal nextValue = slope * count + intercept;

        return nextValue;
    }

    public class Root
    {
        public List<Data> Data { get; set; }
    }

    public class Data
    {
        public string Symbol { get; set; }
        public string Identifier { get; set; }
        public decimal Open { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal LastPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal Change { get; set; }
        public decimal PChange { get; set; }
        public decimal YearHigh { get; set; }
        public decimal YearLow { get; set; }
        public decimal ffmc { get; set; }
        public decimal totalTradedVolume { get; set; }
        
    }
}
