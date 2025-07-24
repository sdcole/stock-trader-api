using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Models;

namespace StockTraderAPI.Controllers
{

    [ApiController]
    [Route("api/trade")]
    public class AskAIController : Controller
    {
        [Required]
        private readonly IConfiguration _configuration;
        public AskAIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /**
         * AskAIController handles requests to the Ask AI endpoint.
         * It will return AI related data based on the provided symbol and request type.
         * 
         * A few steps are required to complete this
         * 1. Get data from the db related to the stock prices
         * 2. Create an AI prompt and call that will ask the LLM for its input on the request.
         **/
        [HttpPost("v1/ask-ai")]
        public async Task<IActionResult> AskAI(AskAIRequest request)
        {
            if (string.IsNullOrEmpty(request.Symbol) || string.IsNullOrEmpty(request.RequestType))
            {
                return BadRequest("Symbol and RequestType cannot be null or empty.");
            }
            HttpClient dataClient = new HttpClient();
            HttpResponseMessage dataResponse = new HttpResponseMessage();
            string prompt = "";
            
                dataResponse = await dataClient.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/daily-bars?symbol=" + request.Symbol + "&startTime=" + DateTime.UtcNow.AddYears(-1).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
            
            string stockData = await dataResponse.Content.ReadAsStringAsync();


            if (string.IsNullOrEmpty(stockData))
            {
                return NotFound("No data found for the specified stock symbol.");
            }

            HttpClient aiClient = new HttpClient();

            if (request.RequestType == "Is this a stable stock?")
            {
                prompt =
                "You are a financial analyst AI. Given the following OHLCV stock data for the past year, classify the stock as STABLE or UNSTABLE.\n" +
                "Classify as UNSTABLE if any of these apply:\n" +
                "- Any single day price change (up or down) greater than 2%.\n" +
                "- Cumulative price drop or rise of 10% or more within any 7-day period.\n" +
                "- Noticeable high volatility or unpredictable price movements.\n" +
                "- Significant fluctuations or sudden changes in trading volume.\n" +
                "Classify as STABLE only if price movements are moderate daily, no large cumulative moves in any week, volatility is low, and volume is consistent.\n\n" +
                "Include in your analysis:\n" +
                "- Max daily price change (%).\n" +
                "- Max 7-day cumulative price change (%).\n" +
                "- Volume stability.\n\n" +
                "Stock Data:\n" +
                $"{stockData}\n\n" +
                "Respond exactly in this one-line format:\n" +
                "Classification: STABLE or UNSTABLE | Reason1; Reason2; Reason3" +
                "\n" +
                "Only respond using the exact format and avoid extra commentary.";

            }
            else if(request.RequestType == "Is the stock going up or down?")
            {
                prompt =
                "You are a financial analyst AI. Given the following OHLCV stock data for the past year, state whether the stock is predicted to go up or down.\n" +
                "Classify as UP if the provided data shows a high likelyhood of going up"+
                "Classify as DOWN if the provided data shows a high likelyhood of going up.\n\n" +
                "Include in your analysis:\n" +
                "- Max daily price change (%).\n" +
                "- Max 7-day cumulative price change (%).\n" +
                "- Volume stability.\n\n" +
                "Stock Data:\n" +
                $"{stockData}\n\n" +
                "Respond exactly in this one-line format:\n" +
                "Classification: UP or DOWN | Reason1; Reason2; Reason3" +
                "\n" +
                "Only respond using the exact format and avoid extra commentary.";
            }

                var payload = new { prompt = prompt };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await aiClient.PostAsync($"{_configuration["BASE_URL"]}/api/ai/v1/prompt-json", httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return Ok(System.Text.Json.JsonDocument.Parse(responseContent).RootElement);
        }
    }

}