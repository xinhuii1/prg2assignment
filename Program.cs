//==========================================================
// Student Number	: S10269252
// Student Name	: Han Xin Hui
// Partner Name	: Belle Chong Qing Xi
//==========================================================

// basic feature 2)	Load files (flights)
// - load the flights.csv file
// - create the Flight objects based on the data loaded
// - add the Flight objects into a Dictionary

using prg2assignment;

// dictionary to store flight objects
Dictionary<string, Flight> flightDict = new Dictionary<string, Flight>();

// method to load the flight.csv flight
void LoadFlights(Dictionary <string, Flight> flightDict)
{
    string[] csvLines = File.ReadAllLines("flights.csv");

    for ( int i = 1;  i < csvLines.Length; i++ ) // skip the header row 
    {
        string[] data = csvLines[i].Split(',');

        if ( data.Length >= 4)
        {
            string flightNumber = data[0];
            string origin = data[1];
            string destination = data[2];
            DateTime expectedTime = DateTime.Parse(data[3]);
            string specialRequestCode = data[4];

            if (specialRequestCode == "CFFT")
            {
                CFFTFlight flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
                flightDict.Add(flightNumber, flight); // add to directory (flight number as the key)
            }

            else if (specialRequestCode == "LWTT")
            {
                LWTTFlight flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500);
                flightDict.Add(flightNumber, flight); // add to directory
            }

            else if (specialRequestCode == "DDJB")
            {
                DDJBFlight flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 250);
                flightDict.Add(flightNumber, flight); // add to directory
            }

            else
            {
                NORMFlight flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                flightDict.Add(flightNumber, flight); // add to directory
            }
        }
    }
}

// main program starts here
LoadFlights(flightDict);

