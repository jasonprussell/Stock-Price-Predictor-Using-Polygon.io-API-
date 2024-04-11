using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Text.Json;

public class Program
{
    static readonly HttpClient client = new HttpClient();

    public class StockData
    {
        public DateTime Date { get; set; }
        public float ClosePrice { get; set; }
    }

    public class StockPrediction
    {
        public float[] ForecastedPrices { get; set; }
        public float[] ConfidenceLowerBound { get; set; }
        public float[] ConfidenceUpperBound { get; set; }
    }

    public class PolygonResponse
    {
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public long T { get; set; } // Timestamp
        public float C { get; set; } // Close price
    }

    static async Task Main(string[] args)
    {
        string apiKey = "YOUR_POLYGON_API_KEY";
        string ticker = "AAPL"; // Example stock ticker
        string apiUrl = $"https://api.polygon.io/v2/aggs/ticker/{ticker}/range/1/day/2000-01-01/2024-04-10?apiKey={apiKey}";

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var stockData = ConvertToStockData(responseBody);

            var mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(stockData);

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                nameof(StockPrediction.ForecastedPrices),
                nameof(StockData.ClosePrice),
                windowSize: 7,
                seriesLength: 8280,
                trainSize: stockData.Count,
                horizon: 7,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: nameof(StockPrediction.ConfidenceLowerBound),
                confidenceUpperBoundColumn: nameof(StockPrediction.ConfidenceUpperBound));

            var forecastingModel = forecastingPipeline.Fit(dataView);

            var forecastEngine = forecastingModel.CreateTimeSeriesEngine<StockData, StockPrediction>(mlContext);
            var forecast = forecastEngine.Predict();

            foreach (var price in forecast.ForecastedPrices)
            {
                Console.WriteLine($"Forecasted price: {price}");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }

    static List<StockData> ConvertToStockData(string responseBody)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var polygonResponse = JsonSerializer.Deserialize<PolygonResponse>(responseBody, options);
        var stockDataList = new List<StockData>();

        if (polygonResponse?.Results != null)
        {
            foreach (var result in polygonResponse.Results)
            {
                var stockData = new StockData
                {
                    Date = DateTimeOffset.FromUnixTimeMilliseconds(result.T).DateTime,
                    ClosePrice = result.C
                };
                stockDataList.Add(stockData);
            }
        }

        return stockDataList;
    }
}
