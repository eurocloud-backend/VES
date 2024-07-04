using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using VES.Models;
using System.Text.Json.Serialization;

namespace VES.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult GetTime()
        {
            var serverTime = new { Time = DateTime.Now.ToString("HH:mm:ss") };
            return Json(serverTime);
        }

        [HttpPost]
        public async Task<IActionResult> VehicleEnquiry([FromBody] VehicleRequest request)
        {
            // add error handling
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var apiKey = _configuration["VehicleApi:ApiKey"]; // Read the API key from configuration
                var apiHost = _configuration["VehicleApi:ApiHost"]; // Read the API host from configuration
                httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
                
                // Create HttpContent with the correct 'Content-Type' header
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiHost, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Ok(responseContent);
                }
                else
                {
                    // Handle error or unsuccessful status code.
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

            }
            catch (Exception ex)
            {

               return StatusCode(500, ex.Message);
            }


        }

        public class VehicleRequest
        {
            // add json property name
            [JsonPropertyName("registrationNumber")]
            public string RegistrationNumber { get; set; }
        }


    }
}
