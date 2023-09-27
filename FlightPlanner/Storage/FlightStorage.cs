using FlightPlanner.Models;

namespace FlightPlanner.Storage
{
    public class FlightStorage
    {
        private static List<Flight> _flightStorage = new List<Flight>();
        private static int _id = 0;

        public bool AddFlight(Flight flight)
        {
            {
                var existingFlight = GetExistingFlight(flight);
                if (existingFlight != null)
                {
                    Console.WriteLine($"Flight already exists: {existingFlight.ID}");
                    return false;
                }

                flight.ID = _id++;
                _flightStorage.Add(flight);
                Console.WriteLine($"Flight added: {flight.ID}");
                return true;
            }
        }

        public List<Flight> GetCopyOfFlightStorage()
        {
            return _flightStorage.ToList();
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
        }

        public Flight? GetFlight(int id)
        {
            if (_flightStorage.Any(flight => flight.ID == id))
            {
                return _flightStorage.FirstOrDefault(flight => flight.ID == id);
            }
            return null;
        }

        public bool DeleteFlight(int id)
        {
            if (_flightStorage == null) return false; 

            int removedCount = _flightStorage.RemoveAll(flight => flight != null && flight.ID == id);
            return removedCount > 0;
        }

        public List<Airport> GetUniqueAirports(string search)
        {
            var searchLower = search.Trim().ToLowerInvariant(); 

            var allAirports = _flightStorage.SelectMany(f => new[] { f.From, f.To })
                                            .Where(a => a.AirportCode.Trim().ToLowerInvariant().Contains(searchLower) ||
                                                        a.City.Trim().ToLowerInvariant().Contains(searchLower) ||
                                                        a.Country.Trim().ToLowerInvariant().Contains(searchLower))
                                            .ToList();

            var uniqueAirports = allAirports
                .GroupBy(a => a.AirportCode)
                .Select(g => g.First())
                .ToList();

            return uniqueAirports;
        }

        public List<Flight> SearchFlights(SearchFlightsRequest request)
        {
            return _flightStorage.Where(f =>
            {
                
                    return f.From.AirportCode == request.From &&
                           f.To.AirportCode == request.To &&
                           f.DepartureTime.Contains(request.DepartureDate);
                
            }).ToList();
        }
    }
}