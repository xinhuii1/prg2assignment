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
    abstract class Flight : IComparable<Flight> 
    {
        // attribute and properties
        private string flightNumber;

        public string FlightNumber
        {
            get { return flightNumber; }
            set
            {
                // validate that flight number is not empty
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Flight Number cannot be empty.");
                flightNumber = value;
            }
        }

        private string origin;

        public string Origin
        {
            get { return origin; }
            set
            {
                // validate that origin is not empty
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Origin cannot be empty.");
                origin = value;
            }
        }

        private string destination;

        public string Destination
        {
            get { return destination; }
            set
            {
                // validate that origin is not empty
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Destination cannot be empty.");
                destination = value;
            }
        }

        private DateTime expectedTime;

        public DateTime ExpectedTime
        {
            get { return expectedTime; }
            set
            {
                if (value == DateTime.MinValue)  // Validate that ExpectedTime is not set to the default value
                    throw new ArgumentException("Expected time cannot be the default DateTime value.");
                expectedTime = value;
            }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string AirlineCode { get; set; }

        //parameterized constructor
        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
        }

        public int CompareTo(Flight other)
        {
            if (this == other) return 1;
            return this.ExpectedTime.CompareTo(other.ExpectedTime);
        }

        // method to calculate the total fees (no longer abstract)
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
            string time = ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower();
            return $"{FlightNumber, -15} {Origin, -25} {Destination, -20} {time}";  // AM/PM is in lower case to match the sample outout
        }
    }
}
