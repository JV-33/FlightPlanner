using System;
namespace FlightPlanner.Models
{
    public class Flight
    {
        public int ID { get; set; }

        public Airport From { get; set; }

        public Airport To { get; set; }

        public string Carrier { get; set; }

        public string DepartureTime { get; set; }

        public string ArrivalTime { get; set; }
    }
}

