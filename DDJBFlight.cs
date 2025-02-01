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
    // DDJBFlight inherits from Flight
    class DDJBFlight : Flight
    {
        // attributes and properties
        private double requestFee;
        public double RequestFee
        {
            get { return requestFee; }
            set
            {
                // Validation: Ensure requestFee is a non-negative value
                if (value < 0)
                    throw new ArgumentException("Request fee cannot be negative.");
                requestFee = value;
            }
        }

        // parameterized constructor
        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        // CalculateFees to include request fee
        public override double CalculateFees()
        {
            return base.CalculateFees() + 300;
        }

        // display 
        public override string ToString()
        {
            return base.ToString();  // same format as base class
        }
    }
}
