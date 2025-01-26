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
    // CFFTFlight inherits from Flight
    class CFFTFlight : Flight
    {
        // attributes and properties
        private double requestFee;

        public double RequestFee
        {
            get { return requestFee; }
            set { requestFee = value; }
        }

        // parameterized constructor
        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        // CalculateFees to include request fee
        public override double CalculateFees()
        {
            return base.CalculateFees() + 150;
        }

        // display 
        public override string ToString()
        {
            return base.ToString();  // same format as base class
        }
    }
}
