using Microsoft.AspNetCore.Mvc;

namespace search_flight_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchFlightController : ControllerBase
    {
        private readonly ILogger<SearchFlightController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public SearchFlightController(ILogger<SearchFlightController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("GetOriginAirports")]
        public async Task<IActionResult> GetOriginAirports()
        {
            _logger.LogInformation("Request for origin airports is received");

            try
            {
                return Ok(new List<string> { "Chennai", "Banglore", "Delhi", "Kolkata","Mumbai" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching origin airports");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetDestinationAirports")]
        public async Task<IActionResult> GetDestinationAirports([FromQuery] string originAirport)
        {
            _logger.LogInformation("Request with origin airport " + originAirport + " is recived");
            if (string.IsNullOrEmpty(originAirport))
            {
                _logger.LogError("Request dont have valid origin airport");
                return BadRequest("Origin airport is needed");
            }

            try
            {
                // With given api, getting 404 error
                //string url = $"https://api-cert.ezycommerce.sabre.com/apj/v1/Airport/OriginsWithConnections/en-us?origin={originAirport}&market=IN&currency=INR";
                //string tenantIdentifier = "9d7d6eeb25cd803e0df323a0fff258e59398a702fac09131275b6b1911e202d";

                //var client = _httpClientFactory.CreateClient();
                //client.DefaultRequestHeaders.Add("Tenant-Identifier", tenantIdentifier);

                //_logger.LogInformation("Calling API with origin" + originAirport);
                //var response = await client.GetAsync(url);

                //if (!response.IsSuccessStatusCode)
                //{
                //    _logger.LogError("API call failed with status" + response.StatusCode);
                //    return StatusCode((int)response.StatusCode, "Error in fetching data from API");
                //}

                //var content = await response.Content.ReadAsStringAsync();

                //_logger.LogInformation("Successfully retrieved destinations for origin " + originAirport);

                //return Content(content, "application/json");

                var routes = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "Mumbai", new List<string> { "Chennai", "Banglore", "Delhi", "Kolkata" } },
                        { "Chennai", new List<string> { "Delhi", "Banglore", "Mumbai", "Kolkata" } },
                        { "Banglore", new List<string> { "Delhi", "Chennai", "Mumbai", "Kolkata" } },
                        { "Delhi", new List<string> { "Mumbai", "Banglore", "Chennai", "Kolkata" } },
                        { "Kolkata", new List<string> { "Delhi", "Banglore", "Mumbai", "Chennai" } }
                    };
                if (!routes.ContainsKey(originAirport))
                {
                    return NotFound("There is no destination pair for origin " + originAirport);
                }
                var test = routes[originAirport];
                return Ok(test);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching destination airports for origin {Origin} airports", originAirport);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
