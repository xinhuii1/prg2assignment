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
        private Dictionary<string, Airline> airlines;
        private Dictionary<string, Flight> flights;
        private Dictionary<string, BoardingGate> boardingGates;
        private Dictionary<string, double> gateFees;

        public Terminal(string terminalName)
        {
            TerminalName = terminalName; 
            airlines = new Dictionary<string, Airline>();
            flights = new Dictionary<string, Flight>();
            boardingGates = new Dictionary<string, BoardingGate>();
            gateFees = new Dictionary<string, double>();
        }

        public Dictionary<string, BoardingGate> BoardingGates
        {
            get { return boardingGates; }
        }

        public bool AddAirline(Airline airline)                  // check whether the code existed, if no add to airlines dict
        {
            if (airlines.ContainsKey(airline.Code)) 
            {
                airlines.Add(airline.Code, airline); 
                return true;
            }

            return false; 
        }

        public bool AddBoardingGate(BoardingGate gate)           // check whether the gate existed, if no add to boardingGate dict
        {
            if (boardingGates.ContainsKey(gate.GateName) == false)
            { 
                boardingGates.Add(gate.GateName, gate);
                return true; 
            }

            return false;
        }

        public Airline GetAirlineFromFlight(string flightNumber)
        {
            foreach (var airline in airlines.Values)               // Loop through all airlines, values represent different airlines
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
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"{airline.Name}: ${airline.CalculateFees():F2}");     // Display the fees of each airline
            }
        }

        public override string ToString()
        {
            return $"Terminal: {TerminalName}, Airlines: {airlines.Count}, Flights: {flights.Count}, Boarding Gates: {boardingGates.Count}";
        }

    }
}
