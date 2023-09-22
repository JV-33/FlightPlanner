using FlightPlanner.Models;
using FlightPlanner.Controllers;


namespace FlightPlanner.Storage
{
    public class FlightStorage
    {
        private static List<Flight> _flightStorage = new List<Flight>();
        private static int _id = 0;

        public void AddFlight(Flight flight)
        {
            flight.ID = _id++;
            _flightStorage.Add(flight);

        }

        public void Clear()
        {
            _flightStorage.Clear();
        }

        public bool FlightExists(int id)
        {
            return _flightStorage.Any(f => f.ID == id);
        }


        public Flight GetExistingFlight(Flight flight)
        {
            return _flightStorage.FirstOrDefault(f =>
                f.From.Country == flight.From.Country &&
                f.From.City == flight.From.City &&
                f.From.AirportCode == flight.From.AirportCode &&
                f.To.Country == flight.To.Country &&
                f.To.City == flight.To.City &&
                f.To.AirportCode == flight.To.AirportCode &&
                f.DepartureTime == flight.DepartureTime
            );
        }

        public Flight? GetFlight(int id)
        {
            if (_flightStorage.Any(flight => flight.ID == id))
            {
                return _flightStorage.First(flight => flight.ID == id);
            }
            return null;
        }

        public bool DeleteFlight(int id)
        {
            if (!FlightExists(id))
            {
                return false;
            }

            var flightToRemove = _flightStorage.First(flight => flight.ID == id);
            _flightStorage.Remove(flightToRemove);
            return true;
        }





        public List<Airport> GetUniqueAirports(string search)
        {
            var searchLower = search.ToLowerInvariant();

            var allAirports = _flightStorage.SelectMany(f => new[] { f.From, f.To })
                                            .Where(a => a.AirportCode.ToLowerInvariant().Contains(searchLower) ||
                                                        a.City.ToLowerInvariant().Contains(searchLower) ||
                                                        a.Country.ToLowerInvariant().Contains(searchLower))
                                            .ToList();

            var uniqueAirports = allAirports
                .GroupBy(a => a.AirportCode)
                .Select(g => g.First())
                .ToList();

            return uniqueAirports;
        }

    }
}

