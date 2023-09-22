using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}


