using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2assignment
{
    internal class BoardingGate
    {
        public string GateName { get; set; }
        public bool SupportsCFFT { get; set; }
        public bool SupportsDDJB { get; set; }
        public bool SupportsLWTT { get; set; }
        public Flight AssignedFlight { get; set; }

        public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT)
        {
            GateName = gateName;
            SupportsCFFT = supportsCFFT;
            SupportsDDJB = supportsDDJB;
            SupportsLWTT = supportsLWTT;
            AssignedFlight = null;                                // Initially, no flight is assigned
        }

        public double CalculateFees()
        {
            double baseFee = 300; 
            double requestFee = 0;

            if (AssignedFlight != null )
            {
                if (SupportsCFFT == true)
                {
                    requestFee = 150;
                }
                if (SupportsDDJB == true)
                {
                    requestFee = 300;
                }
                if (SupportsLWTT == true)
                {
                    requestFee = 500;
                }

                return baseFee + requestFee;
            }

            return baseFee;
        }

        public override string ToString()
        {
            return $"{GateName}\t{SupportsDDJB}\t{SupportsCFFT}\t{SupportsLWTT}";
        }
    }
}
