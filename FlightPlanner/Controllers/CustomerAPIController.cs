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

        public CustomerAPIController(FlightStorage storage)
        {
            _storage = storage;
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
        public IActionResult SearchFlights([FromBody] SearchFlightsRequest request)
        {
            if (string.IsNullOrEmpty(request.From.AirportCode) || string.IsNullOrEmpty(request.To.AirportCode) || request.Date == default)
            {
                return BadRequest(new { Error = "Invalid Request: Missing required fields." });
            }

            var flights = _storage.SearchFlights(request);

            return Ok(new { page = 0, totalItems = flights?.Count ?? 0, items = flights ?? new List<Flight>() });

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
    }
}