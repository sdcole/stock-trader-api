using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace StockTraderAPI.Controllers
{
    public class LineGraphController : Controller
    {
        [HttpGet("v1/line-graph")]
        public async Task<IActionResult> GetLineGraphData(string ticker, string timeframe)
        {
            using var client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            switch(timeframe)
            {
                case "1d":
                    response = await client.GetAsync($"http://localhost:5119/v1/minute-bars?ticker=" + ticker + "&startTime=" + DateTime.UtcNow.AddDays(-1).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                    break;

                case "7d":
                    response = await client.GetAsync($"http://localhost:5119/v1/daily-bars?ticker=" + ticker + "&startTime=" + DateTime.UtcNow.AddDays(-7).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                    break;

                case "1m":
                    response = await client.GetAsync($"http://localhost:5119/v1/daily-bars?ticker=" + ticker + "&startTime=" + DateTime.UtcNow.AddDays(-30).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                    break;

                case "3m":
                    response = await client.GetAsync($"http://localhost:5119/v1/daily-bars?ticker=" + ticker + "&startTime=" + DateTime.UtcNow.AddDays(-90).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                    break;

                case "6m":
                    response = await client.GetAsync($"http://localhost:5119/v1/daily-bars?ticker=" + ticker + "&startTime=" + DateTime.UtcNow.AddDays(-180).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                    break;
            }
            

            return StatusCode(200, await response.Content.ReadAsStringAsync());
        }
    }
}
