using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        private readonly FlightStorage _storage;
        private readonly ILogger<CustomerAPIController> _logger;

        public CustomerAPIController(FlightStorage storage, ILogger<CustomerAPIController> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search term is missing or empty");

            var airports = _storage.GetUniqueAirports(search);
            if (airports == null || !airports.Any())
                return NotFound("No airports found");

            return Ok(airports);
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult SearchFlights(SearchFlightsRequest request)
        {
            _logger.LogInformation("SearchFlights endpoint hit with From: {From} and To: {To}", request.From, request.To);

            if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To) || request.From == request.To)
            {
                _logger.LogWarning("Invalid Request: Missing required fields. From: {From}, To: {To}", request.From, request.To);
                return BadRequest(new { Error = "Invalid Request: Missing required fields." });
            }

            try
            {
                var flights = _storage.SearchFlights(request);
                return Ok(new { page = 0, totalItems = flights?.Count ?? 0, items = flights ?? new List<Flight>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching flights with From: {From} and To: {To}", request.From, request.To); 
                throw;
            }
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult FindFlightById(int id)
        {
            var flight = _storage.GetFlight(id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        [HttpGet]
        [Route("flights")]
        public IActionResult GetAllFlights()
        {
            var flights = _storage.GetCopyOfFlightStorage();
            Console.WriteLine($"Number of flights: {flights.Count}");
            if (flights == null || !flights.Any())
            {
                return NotFound("No flights found");
            }
            return Ok(flights);
        }
    }
}