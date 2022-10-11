using Microsoft.AspNetCore.Mvc;
using Redis_AspNet.Attributes;
using Redis_AspNet.Services;

namespace Redis_AspNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IReposeCacheService responseService;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger, IReposeCacheService responseService)
        {
            _logger = logger;
            this.responseService = responseService;
        }



        // WeatherForecast/getall
        [HttpGet("GetAll")]
        [Cache(1000)]
        public ActionResult<IEnumerable<WeatherForecast>> GetAsync
            (string keyword = null, int pageIndex = 1, int pageSize = 20)
        {
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await responseService.RemoveCacheResponseAsync("/WeatherForecast/GetAll");

            return Ok();
        }

    }
}