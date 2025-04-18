﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace StockTraderAPI.Controllers
{
    [ApiController]
    [Route("api/trade")]
    public class LineGraphController : Controller
    {
        [Required]
        private readonly IConfiguration _configuration;

        public LineGraphController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("v1/line-graph")]
        public async Task<IActionResult> GetLineGraphData(string symbol, string timeframe)
        {
            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                switch (timeframe)
                {
                    case "1d":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/minute-bars?symbol=" + symbol + "&startTime=" + DateTime.UtcNow.AddDays(-1).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;

                    case "7d":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/minute-bars?symbol=" + symbol + "&startTime=" + DateTime.UtcNow.AddDays(-7).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;

                    case "1m":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/minute-bars?symbol=" + symbol + "&interval=1h&startTime=" + DateTime.UtcNow.AddDays(-30).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;

                    case "3m":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/minute-bars?symbol=" + symbol + "&interval=1h&startTime=" + DateTime.UtcNow.AddMonths(-1).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;

                    case "6m":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/daily-bars?symbol=" + symbol + "&startTime=" + DateTime.UtcNow.AddMonths(-6).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;

                    case "1y":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/daily-bars?symbol=" + symbol + "&startTime=" + DateTime.UtcNow.AddYears(-1).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;
                    case "5y":
                        response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/daily-bars?symbol=" + symbol + "&startTime=" + DateTime.UtcNow.AddYears(-5).ToUniversalTime() + "&endTime=" + DateTime.UtcNow);
                        break;
                }


                return StatusCode(200, await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Error Occurred.");
            }
            
        }
    }
}
