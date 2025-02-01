//==========================================================
// Student Number	: S10269252
// Student Name	: Han Xin Hui
// Partner Name	: Belle Chong Qing Xi
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2assignment
{
    // NORMFlight inherits from Flight
    class NORMFlight : Flight 
    {
        // parameterized constructor 
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status) 
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            // Validate flight number, origin, and destination before setting them
            if (string.IsNullOrEmpty(flightNumber))
            {
                throw new ArgumentException("Flight number cannot be empty.");
            }

            if (string.IsNullOrEmpty(origin))
            {
                throw new ArgumentException("Origin cannot be empty.");
            }

            if (string.IsNullOrEmpty(destination))
            {
                throw new ArgumentException("Destination cannot be empty.");
            }

            if (expectedTime == DateTime.MinValue)
            {
                throw new ArgumentException("Invalid expected time for the flight.");
            }
        }

        public override double CalculateFees()
        {
            return base.CalculateFees();
        }

        // display
        public override string ToString()
        {
            return base.ToString(); // same format as base class
        }
    }
}
