using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StockTraderAPI.Controllers
{
    [ApiController]
    [Route("api/trade")]
    public class DataGridController : Controller
    {
        [Required]
        private readonly IConfiguration _configuration;
        public DataGridController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("v1/data-grid")]
        public async Task<IActionResult> GetDataGridData()
        {
            try
            {
                using var client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                response = await client.GetAsync($"{_configuration["BASE_URL"]}/api/market/v1/companies");
                return StatusCode(200, await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Error Occurred.");
            }

        }
    }
}
