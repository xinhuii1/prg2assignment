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
    // LWWTFlight inherits from Flight
    class LWTTFlight : Flight
    {
        // attributes and properties
        private double requestFee;

        // property
        public double RequestFee
        {
            get { return requestFee; }
            set
            {

            // Validation to ensure requestFee is a positive number
            if (value < 0)
                throw new ArgumentException("Request fee cannot be negative.");
            requestFee = value;

            }
        }

        // parameterized constructor
        public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        // CalculateFees to include request fee
        public override double CalculateFees()
        {
            return base.CalculateFees() + 500;
        }

        // display
        public override string ToString()
        {
            return base.ToString();  // same format as base class
        }
    }
}
