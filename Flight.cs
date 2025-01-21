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
    abstract class Flight
    {
        // attribute
        private string flightNumber;

        // property
        public string FlightNumber
        {
            get { return flightNumber; }
            set { flightNumber = value; }
        }

        private string origin;

        public string Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        private string destination;

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        private DateTime expectedTime;

        public DateTime ExpectedTime
        {
            get { return expectedTime; }
            set { expectedTime = value; }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        //parameterized constructor
        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
        }

        // method to calculate the total fees
        public virtual double CalculateFees()
        {
            double boardingFee = 300; // base fee for all boarding gates

            // boarding fees based on the origin and destination
            if (Destination == "Singapore (SIN)")
            {
                boardingFee += 500; // arriving flight (SIN)
            }

            if (Origin == "Singapore (SIN)")
            {
                boardingFee += 800; // departing flight (SIN)
            }

            return boardingFee;
        }

        // display flight details
        public virtual string ToString()
        {
            return $"{FlightNumber, -15} {Origin, -25} {Destination, -25} {ExpectedTime:dd/MM/yyyy hh:mm tt}";
        }
    }
}
