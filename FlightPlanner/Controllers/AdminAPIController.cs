using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlightPlanner.Storage;


namespace FlightPlanner.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminAPIController : ControllerBase
    {
        private readonly FlightStorage _storage;


        public AdminAPIController()
        {
            _storage = new FlightStorage();
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = _storage.GetFlight(id);
            if (flight == null)
            {
                return NotFound();
            }
            return Ok(flight);
        }


        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(Flight flight)
        {
            if (flight == null)
                return BadRequest("Missing flight information");

            if (flight.From == null || string.IsNullOrEmpty(flight.From.Country) || string.IsNullOrEmpty(flight.From.City) || string.IsNullOrEmpty(flight.From.AirportCode))
                return BadRequest("Incomplete 'from' airport information");

            if (flight.To == null || string.IsNullOrEmpty(flight.To.Country) || string.IsNullOrEmpty(flight.To.City) || string.IsNullOrEmpty(flight.To.AirportCode))
                return BadRequest("Incomplete 'to' airport information");

            if (string.IsNullOrEmpty(flight.Carrier))
                return BadRequest("Missing carrier information");

            if (string.IsNullOrEmpty(flight.DepartureTime))
                return BadRequest("Missing departure time");

            if (string.IsNullOrEmpty(flight.ArrivalTime))
                return BadRequest("Missing arrival time");
            if (flight.From.AirportCode.Trim().Equals(flight.To.AirportCode.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest("Departure and arrival airports must be different");

            DateTime departureTime, arrivalTime;
            if (!DateTime.TryParse(flight.DepartureTime, out departureTime) || !DateTime.TryParse(flight.ArrivalTime, out arrivalTime))
                return BadRequest("Invalid time format");

            if (arrivalTime <= departureTime)
                return BadRequest("Arrival time must be later than departure time");

            var existingFlight = _storage.GetExistingFlight(flight);
            if (existingFlight != null)
                return Conflict("A flight with the same data already exists");

            _storage.AddFlight(flight);
            return Created(" ", flight);
        }




        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            _storage.DeleteFlight(id);
            return Ok();
        }



    }
}