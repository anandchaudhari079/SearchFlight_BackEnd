using Microsoft.AspNetCore.Mvc;
using search_flight_backend.Services;

namespace search_flight_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchFlightController : ControllerBase
    {
        private readonly ILogger<SearchFlightController> _logger;
        private readonly IAirportService _airportService;

        public SearchFlightController(ILogger<SearchFlightController> logger, IAirportService airportService)
        {
            _logger = logger;
            _airportService = airportService;
        }

        [HttpGet("GetOriginAirports")]
        public async Task<IActionResult> GetOriginAirports()
        {
            _logger.LogInformation("Request for origin airports is received");
            try
            {
                var airports = await _airportService.GetOriginAirportsAsync();
                return Ok(airports);
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
            _logger.LogInformation("Request with origin airport {OriginAirport} is received", originAirport);
            if (string.IsNullOrEmpty(originAirport))
            {
                _logger.LogError("Request doesn't have valid origin airport");
                return BadRequest("Origin airport is needed");
            }

            try
            {
                var destinations = await _airportService.GetDestinationAirportsAsync(originAirport);
                if (destinations == null)
                {
                    return NotFound("There is no destination pair for origin " + originAirport);
                }
                return Ok(destinations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching destination airports for origin {Origin} airports", originAirport);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}