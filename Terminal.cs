using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2assignment
{
    class Terminal
    {
        public string TerminalName { get; set; } 
        private Dictionary<string, BoardingGate> boardingGates;
        private Dictionary<string, double> gateFees;

        public Dictionary<string, Airline> Airlines { get; set; }

        public Dictionary<string, Flight> Flights { get; set; }
        public Dictionary<string, BoardingGate> BoardingGates { get; set; }


        public Terminal(string terminalName)
        {
            TerminalName = terminalName; 
            Airlines = new Dictionary<string, Airline>();
            Flights = new Dictionary<string, Flight>();
            BoardingGates = new Dictionary<string, BoardingGate>();
            gateFees = new Dictionary<string, double>();
        }


        public bool AddAirline(Airline airline)
        {
            if (!Airlines.ContainsKey(airline.Code))
            {
                Airlines.Add(airline.Code, airline);
                return true;
            }
            return false;
        }

        public bool AddBoardingGate(BoardingGate gate)
        {
            if (!BoardingGates.ContainsKey(gate.GateName))
            {
                BoardingGates.Add(gate.GateName, gate);
                return true;
            }
            return false;
        }

        public Airline GetAirlineFromFlight(string flightNumber)
        {
            foreach (var airline in Airlines.Values)               // Loop through all airlines, values represent different airlines
            {
                if (airline.Flights.ContainsKey(flightNumber)) // Check if the flight number exists in the airline'sdict
                {
                    return airline;                                // Return the matching airline
                }
            }

            return null; 
        }

        public void PrintAirlineFees()
        {
            foreach (var airline in Airlines.Values)
            {
                Console.WriteLine($"{airline.Name}: ${airline.CalculateFees():F2}");     // Display the fees of each airline
            }
        }

 

        public override string ToString()
        {
            return $"Terminal: {TerminalName}, Airlines: {Airlines.Count}, Flights: {Flights.Count}, Boarding Gates: {boardingGates.Count}";
        }

    }
}
