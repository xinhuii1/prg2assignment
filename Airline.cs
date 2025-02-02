using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2assignment
{
    class Airline
    {
        public string Name { get; set; }                                // Properties
        public string Code { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }         

        public Airline(string name, string code)                        // Constructor
        {
            Name = name;
            Code = code;
            Flights = new Dictionary<string, Flight>();                 // Each airline has its own dict
        }

        public bool AddFlight(Flight flight)

        {
            if (Flights.ContainsKey(flight.FlightNumber))               // Check if the flight already exists in the dictionary
            {
                return false;
            }
            Flights.Add(flight.FlightNumber, flight);                   // Add the flight to dict
            return true;
        }

        public bool RemoveFlight(string flightNumber)
        {
            if (Flights.ContainsKey(flightNumber))
            {
                Flights.Remove(flightNumber);                           // Remove the flightNumber and respective values
                Console.WriteLine($"Flight {flightNumber} removed from dictionary.");
                return true;
            }
            else
            {
                Console.WriteLine($"Flight {flightNumber} does not exist in the dictionary.");
                return false;
            }
        }
        public double CalculateFees()                                 // Calculate the total amount earned in an airline
        {
            double totalFees = 0;
            foreach (var flight in Flights.Values)
            {
                totalFees += flight.CalculateFees();
            }
            return totalFees;
        }

        public override string ToString()
        {
            return $"{Code,-15}{Name,-25}";

        }
    }
}
