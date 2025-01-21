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

        }

        public override double CalculateFees()
        {
            return base.CalculateFees();
        }

        public override string ToString()
        {
            return base.ToString(); // same format as base class
        }
    }
}
